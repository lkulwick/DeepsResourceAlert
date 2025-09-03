using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ResourceAlert
{
    [StaticConstructorOnStartup]
    public class SetResourcesWindow : Window
    {
        private bool AcceptsInput
        {
            get
            {
                return this.startAcceptingInputAtFrame <= Time.frameCount;
            }
        }
        protected virtual int MaxCountLength
        {
            get
            {
                return 28;
            }
        }
        public override Vector2 InitialSize => new Vector2(340f, 220f);



        public SetResourcesWindow(ThingDef resource)
        {
            this.alertableResource = resource;
            this.alertableCategory = null;
            InitWindow();

            if (resource != null && ResourceAlertManager.Resources.TryGetValue(resource, out var value))
                curLimit = value.ToString();
            else
                curLimit = "0";

            DebugLog.Message("windowstack constructor resource: " + resource.defName);
        }

        public SetResourcesWindow(ThingCategoryDef category)
        {
            this.alertableCategory = category;
            this.alertableResource = null;
            InitWindow();

            if (category != null && ResourceAlertManager.Categories.TryGetValue(category, out var value))
                curLimit = value.ToString();
            else
                curLimit = "0";

            DebugLog.Message("windowstack constructor category: " + category.defName);
        }

        private void InitWindow()
        {
            this.forcePause = false;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = true;
        }




        //public SetResourcesWindow(ThingCategoryDef resource)
        //{
        //	this.forcePause = false;
        //	this.doCloseX = true;
        //	this.absorbInputAroundWindow = true;
        //	this.closeOnAccept = false;
        //	this.closeOnClickedOutside = true;
        //	this.alertableResource = resource;

        //	DebugLog.Message("windowstack contructor. res: " + resource.defName);
        //}
        public void WasOpenedByHotkey()
        {
            this.startAcceptingInputAtFrame = Time.frameCount + 1;
        }
        protected virtual AcceptanceReport ValueIsValid(string name)
        {
            if (name.Length == 0 || !int.TryParse(name, out _))
            {
                return false;
            }
            return true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            float curY = 0f;
            float spacing = 10f;

			bool enterPressed = 
                KeyBindingDefOf.Accept.KeyDownEvent || 
                (Event.current.type == EventType.KeyDown &&
                (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter));


			// --- "Track resource:" title ---
			Text.Font = GameFont.Medium;
            string prefix = "Track resource:";
            Vector2 prefixSize = Text.CalcSize(prefix);

            Texture iconTex = alertableResource?.uiIcon ?? alertableCategory?.icon ?? BaseContent.BadTex;
            float iconSize = 24f;
            float titleLineHeight = Mathf.Max(prefixSize.y, iconSize);
            float totalWidth = prefixSize.x + 6f + iconSize;
            float x = (inRect.width - totalWidth) / 2f;
            float centerY = titleLineHeight / 2f;

            Rect prefixRect = new Rect(x, 0f + centerY - prefixSize.y / 2f, prefixSize.x, prefixSize.y);
            Rect iconRect = new Rect(prefixRect.xMax + 6f, 0f + centerY - iconSize / 2f, iconSize, iconSize);


			// Draw prefix and icon
			Widgets.Label(prefixRect, prefix);
            GUI.DrawTexture(iconRect, iconTex);

            // Tooltip with full name
            string label = alertableResource?.LabelCap ?? alertableCategory?.LabelCap ?? "???";
            TooltipHandler.TipRegion(iconRect, label);

            curY += titleLineHeight + spacing;
            Text.Font = GameFont.Small;



            // --- Input field (same width as button) ---
            float controlWidth = inRect.width - 30f;
            Rect textFieldRect = new Rect(15f, curY, controlWidth, 30f);
            GUI.SetNextControlName("TextField_SetResourceLimit");

            string newText = Widgets.TextField(textFieldRect, curLimit);
            if (AcceptsInput && newText.Length < MaxCountLength)
                curLimit = newText;
            else if (!AcceptsInput)
                ((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).SelectAll();

            if (!focused_TextResourceLimitTextField)
            {
                UI.FocusControl("TextField_SetResourceLimit", this);
                focused_TextResourceLimitTextField = true;
            }

            curY += 30f + spacing;

            // --- Track button (was: Set Alert) ---
            float buttonHeight = 35f;
            Rect buttonRect = new Rect(15f, curY, controlWidth, buttonHeight);
            bool pressed = Widgets.ButtonText(buttonRect, "Track");

			// If the text field is focused, we still want Enter to trigger Track.
			if (enterPressed)
			{
				// Consume the event so it doesn't bubble or trigger other windows.
				Event.current.Use();
				pressed = true;
			}


			if (pressed)
            {
                AcceptanceReport acceptanceReport = ValueIsValid(curLimit);
                if (!acceptanceReport.Accepted)
                {
                    Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
                    return;
                }

                if (int.TryParse(curLimit, out int limit))
                {
                    if (limit != 0)
                    {
                        if (alertableResource != null)
                        {
                            ResourceChecker.AddAlertableResource(alertableResource, limit);
                            Messages.Message("Added resource " + alertableResource.LabelCap, MessageTypeDefOf.PositiveEvent);
                        }
                        else if (alertableCategory != null)
                        {
                            ResourceChecker.AddAlertableCategory(alertableCategory, limit);
                            Messages.Message("Added category " + alertableCategory.LabelCap, MessageTypeDefOf.PositiveEvent);
                        }
                    }
                    else
                    {
                        if (alertableResource != null)
                        {
                            ResourceChecker.RemoveAlertableResource(alertableResource);
                            Messages.Message("Removed resource " + alertableResource.LabelCap, MessageTypeDefOf.PositiveEvent);
                        }
                        else if (alertableCategory != null)
                        {
                            ResourceChecker.RemoveAlertableCategory(alertableCategory);
                            Messages.Message("Removed category " + alertableCategory.LabelCap, MessageTypeDefOf.PositiveEvent);
                        }
                    }

                    Find.WindowStack.TryRemove(this, true);
                }
            }

            // --- Show Tracked List link ---
            Vector2 linkSize = Text.CalcSize("Show Tracked List");
            float margin = 10f;
            Rect linkRect = new Rect(
                inRect.width - linkSize.x - margin,
                inRect.height - linkSize.y - margin,
                linkSize.x,
                linkSize.y
            );

            GUI.color = new Color(0.6f, 0.6f, 1f); // light blue
            Widgets.Label(linkRect, "Show Tracked List");
            GUI.color = Color.white;

            if (Widgets.ButtonInvisible(linkRect, true))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                Find.WindowStack.Add(new TrackedResourcesWindow());
            }

            if (Mouse.IsOver(linkRect))
            {
                Widgets.DrawLineHorizontal(linkRect.x, linkRect.yMax, linkRect.width);
                GUI.DrawTexture(linkRect, TexUI.HighlightTex);
            }
        }

		protected string curLimit = "0";
        private bool focused_TextResourceLimitTextField;
        private int startAcceptingInputAtFrame;
        private ThingDef alertableResource;
        private ThingCategoryDef alertableCategory;
    }
}
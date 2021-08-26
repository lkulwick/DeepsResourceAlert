using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

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
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(280f, 175f);
            }
        }
        public SetResourcesWindow(ThingDef resource)
        {
            this.forcePause = false;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = true;
            this.alertableResource = resource;

            Log.Message("windowstack contructor. res: " + resource.defName);
        }
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
            Text.Font = GameFont.Small;
            bool flag = false;
            string TextFieldName = "TextField_SetResourceLimit";
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                flag = true;
                Event.current.Use();
            }
            GUI.SetNextControlName(TextFieldName);
            string text = Widgets.TextField(new Rect(0f, 15f, inRect.width, 35f), this.curLimit);
            if (this.AcceptsInput && text.Length < this.MaxCountLength)
            {
                this.curLimit = text;
            }
            else if (!this.AcceptsInput)
            {
                ((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).SelectAll();
            }
            if (!this.focused_TextResourceLimitTextField)
            {
                UI.FocusControl(TextFieldName, this);
                this.focused_TextResourceLimitTextField = true;
            }
            if (Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK", true, true, true) || flag)
            {
                AcceptanceReport acceptanceReport = this.ValueIsValid(this.curLimit);
                if (!acceptanceReport.Accepted)
                {
                    Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
                    return;
                }
                int alertResourceLimit = 0;
                int.TryParse(curLimit, out alertResourceLimit);
                if (alertResourceLimit != 0)
                {
                    ResourceChecker.AddAlertableResource(alertableResource, alertResourceLimit);
                    Messages.Message("Added resource " + alertableResource.defName, MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    ResourceChecker.RemoveAlertableResource(alertableResource);
                    Messages.Message("Removed resource " + alertableResource.defName, MessageTypeDefOf.PositiveEvent);
                }
                Find.WindowStack.TryRemove(this, true);

            }
        }

        protected string curLimit = "0";
        private bool focused_TextResourceLimitTextField;
        private int startAcceptingInputAtFrame;
        private ThingDef alertableResource;
    }
}
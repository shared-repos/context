using System;
using System.Collections.Generic;
using System.Drawing;
using Context.Interfaces.UI.CommandBars;

namespace Context.Core
{
    internal class Command : ICommand
    {
        private string commandName;
        private CommandGroup commandGroup;
        private string displayText;
        private string toolTipText;
        private int imageId;
        private Image image;
        private Guid contextId;
        private CommandBarControlType controlType;
        private CommandBarControlBehavior behavior;
        private List<string> items;
        private bool m_checked;
        private bool m_enabled;
        private bool m_visible;
        private string m_Value;
        private int m_SelectedIndex;

        public Command(string commandName, CommandGroup commandGroup, string displayText, string toolTipText, int imageId, Image image, CommandBarControlType controlType, CommandBarControlBehavior behavior, Guid contextId)
        {
            this.commandName = commandName;
            this.commandGroup = commandGroup;
            this.displayText = displayText;
            this.toolTipText = toolTipText;
            this.imageId = imageId;
            this.image = image;
            this.controlType = controlType;
            this.behavior = behavior;
            this.contextId = contextId;
            this.items = new List<string>();
            if ((behavior & CommandBarControlBehavior.DefaultDisabled) == 0)
            {
                m_enabled = true;
            }
            if ((behavior & CommandBarControlBehavior.DefaultInvisible) == 0)
            {
                m_visible = true;
            }
            if ((behavior & CommandBarControlBehavior.DefaultChecked) != 0)
            {
                m_checked = true;
            }
        }

        #region ICommand Members

        public CommandBarControlBehavior Behavior
        {
            get
            {
                return behavior;
            }
        }

        public CommandBarControlType ControlType
        {
            get
            {
                return controlType;
            }
        }

        public Guid ContextId
        {
            get
            {
                return contextId;
            }
        }

        public bool Checked
        {
            get
            {
                return m_checked;
            }
            set
            {
                m_checked = value;
            }
        }

        public IList<string> Items
        {
            get
            {
                return items;
            }
        }

        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }

        public CommandGroup CommandGroup
        {
            get
            {
                return commandGroup;
            }
        }

        public string Group
        {
            get
            {
                return commandGroup.Name;
            }
        }

        public Image Icon
        {
            get
            {
                return image;
            }
        }

        public int ImageId
        {
            get
            {
                return imageId;
            }
        }

        public string Name
        {
            get
            {
                return commandName;
            }
        }

        public string Text
        {
            get
            {
                return displayText;
            }
            set
            {
                displayText = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return toolTipText;
            }
            set
            {
                toolTipText = value;
            }
        }

        public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
            }
        }

        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return m_SelectedIndex;
            }
            set
            {
                m_SelectedIndex = value;
            }
        }

        #endregion

    }
}

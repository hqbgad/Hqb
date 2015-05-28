﻿using System;

namespace Activator.Items.Defensives
{
    class _3364 : item
    {
        internal override int Id
        {
            get { return 3364; }
        }

        internal override string Name
        {
            get { return "Oracles"; }
        }

        internal override string DisplayName
        {
            get { return "神谕"; }
        }

        internal override int Cooldown
        {
            get { return 75000; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Stealth, MenuType.ActiveCheck }; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
            {
                return;
            }
        }
    }
}

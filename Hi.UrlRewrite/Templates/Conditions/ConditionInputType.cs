using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class ConditionInputType : CustomItem
    {

        public static readonly string TemplateId = "{01A28131-5CC9-4419-BD14-351922CE9416}";


        #region Boilerplate CustomItem Code

        public ConditionInputType(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator ConditionInputType(Item innerItem)
        {
            return innerItem != null ? new ConditionInputType(innerItem) : null;
        }

        public static implicit operator Item(ConditionInputType customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        #endregion //Field Instance Methods
    }
}
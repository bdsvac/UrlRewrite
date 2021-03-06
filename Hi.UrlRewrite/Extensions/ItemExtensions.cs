﻿using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Action;
using Hi.UrlRewrite.Templates.Action.Base;
using Hi.UrlRewrite.Templates.Conditions;
using Hi.UrlRewrite.Templates.MatchUrl;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite
{
    public static class ItemExtensions
    {
        public static InboundRule ToInboundRule(this InboundRuleItem inboundRuleItem, IEnumerable<ConditionItem> conditionItems = null, Condition siteCondition = null)
        {

            if (inboundRuleItem == null)
            {
                return null;
            }

            var inboundRule = new InboundRule
            {
                ItemId = inboundRuleItem.ID.Guid,
                Name = inboundRuleItem.Name,
                IgnoreCase = inboundRuleItem.IgnoreCase.Checked,
                Pattern = inboundRuleItem.Pattern.Value,
                Enabled = inboundRuleItem.Enabled.Checked
            };

            var requestedUrlItem = inboundRuleItem.RequestedUrl.TargetItem;
            RequestedUrl? requestedUrlType = null;
            if (requestedUrlItem != null)
            {
                var requestUrlItemId = requestedUrlItem.ID.ToString();
                switch (requestUrlItemId)
                {
                    case Constants.RequestedUrlType_MatchesThePattern_ItemId:
                        requestedUrlType = RequestedUrl.MatchesThePattern;
                        break;
                    case Constants.RequestedUrlType_DoesNotMatchThePattern_ItemId:
                        requestedUrlType = RequestedUrl.DoesNotMatchThePattern;
                        break;
                    default:
                        break;
                }
            }
            inboundRule.RequestedUrl = requestedUrlType;

            var logicalGroupingItem = inboundRuleItem.LogicalGrouping.TargetItem;
            LogicalGrouping? logicalGroupingType = null;
            if (logicalGroupingItem != null)
            {
                var logicalGroupingItemId = logicalGroupingItem.ID.ToString();
                switch (logicalGroupingItemId)
                {
                    case Constants.LogicalGroupingType_MatchAll_ItemId:
                        logicalGroupingType = LogicalGrouping.MatchAll;
                        break;
                    case Constants.LogicalGroupingType_MatchAny_ItemId:
                        logicalGroupingType = LogicalGrouping.MatchAny;
                        break;
                    default:
                        break;
                }
            }
            inboundRule.LogicalGrouping = logicalGroupingType;

            var usingItem = inboundRuleItem.Using.TargetItem;
            Using? usingType = null;
            if (usingItem != null)
            {
                var usingItemId = usingItem.ID.ToString();
                switch (usingItemId)
                {
                    case Constants.UsingType_RegularExpressions_ItemId:
                        usingType = Using.RegularExpressions;
                        break;
                    case Constants.UsingType_Wildcards_ItemId:
                        usingType = Using.Wildcards;
                        break;
                    case Constants.UsingType_ExactMatch_ItemId:
                        usingType = Using.ExactMatch;
                        break;
                    default:
                        break;
                }
            }
            inboundRule.Using = usingType;

            var baseActionItem = inboundRuleItem.Action.TargetItem;
            BaseAction baseAction = null;
            if (baseActionItem != null)
            {
                if (baseActionItem.TemplateID.ToString().Equals(Constants.RedirectActionTemplateId, StringComparison.InvariantCultureIgnoreCase))
                {
                    baseAction = new RedirectItem(baseActionItem).ToRedirectAction();
                }
            }
            inboundRule.Action = baseAction;

            var conditions = new List<Condition>();

            if (conditionItems != null)
            {
                conditions = conditionItems
                    .Select(e => e.ToCondition())
                    .Where(e => e != null)
                    .ToList();
            }

            inboundRule.Conditions = conditions;

            if (siteCondition != null)
            {
                inboundRule.Conditions.Add(siteCondition);
                inboundRule.LogicalGrouping = LogicalGrouping.MatchAll;
            }

            return inboundRule;
        }

        public static RedirectAction ToRedirectAction(this RedirectItem redirectItem)
        {

            if (redirectItem == null)
            {
                return null;
            }

            var redirectTo = redirectItem.BaseRedirectAction.RewriteUrl;
            string actionRewriteUrl;
            Guid? redirectItemId;
            string redirectItemAnchor;

            RulesEngine.GetRedirectUrlOrItemId(redirectTo, out actionRewriteUrl, out redirectItemId, out redirectItemAnchor);

            var redirectAction = new RedirectAction
            {
                Name = redirectItem.Name,
                AppendQueryString = redirectItem.BaseRedirectAction.AppendQueryString.Checked,
                RewriteUrl = actionRewriteUrl,
                RewriteItemId = redirectItemId,
                RewriteItemAnchor = redirectItemAnchor,
                StopProcessingOfSubsequentRules = redirectItem.BaseRedirectAction.BaseStopProcessingAction.StopProcessingOfSubsequentRules.Checked
            };

            var redirectTypeItem = redirectItem.RedirectType.TargetItem;
            RedirectType? redirectType = null;
            if (redirectTypeItem != null)
            {
                switch (redirectTypeItem.ID.ToString())
                {
                    case Constants.RedirectType_Permanent_ItemId:
                        redirectType = RedirectType.Permanent;
                        break;
                    case Constants.RedirectType_Found_ItemId:
                        redirectType = RedirectType.Found;
                        break;
                    case Constants.RedirectType_SeeOther_ItemId:
                        redirectType = RedirectType.SeeOther;
                        break;
                    case Constants.RedirectType_Temporary_ItemId:
                        redirectType = RedirectType.Temporary;
                        break;
                    default:
                        break;
                }
            }
            redirectAction.RedirectType = redirectType;

            return redirectAction;
        }


        public static Condition ToCondition(this ConditionItem conditionItem)
        {

            if (conditionItem == null)
            {
                return null;
            }

            var condition = new Condition
            {
                Name = conditionItem.Name,
                Pattern = conditionItem.Pattern.Value,
                IgnoreCase = conditionItem.IgnoreCase.Checked
            };

            var checkIfInputStringItem = conditionItem.CheckIfInputString.TargetItem;
            CheckIfInputStringType? checkIfInputStringType = null;
            if (checkIfInputStringItem != null)
            {
                switch (checkIfInputStringItem.ID.ToString())
                {
                    case Constants.CheckIfInputStringType_IsAFile_ItemId:
                        checkIfInputStringType = CheckIfInputStringType.IsAFile;
                        break;
                    case Constants.CheckIfInputStringType_IsNotAFile_ItemId:
                        checkIfInputStringType = CheckIfInputStringType.IsNotAFile;
                        break;
                    case Constants.CheckIfInputStringType_IsADirectory_ItemId:
                        checkIfInputStringType = CheckIfInputStringType.IsADirectory;
                        break;
                    case Constants.CheckIfInputStringType_IsNotADirectory_ItemId:
                        checkIfInputStringType = CheckIfInputStringType.IsNotADirctory;
                        break;
                    case Constants.CheckIfInputStringType_MatchesThePattern_ItemId:
                        checkIfInputStringType = CheckIfInputStringType.MatchesThePattern;
                        break;
                    case Constants.CheckIfInputStringType_DoesNotMatchThePattern_ItemId:
                        checkIfInputStringType = CheckIfInputStringType.DoesNotMatchThePattern;
                        break;
                    default:
                        break;
                }
            }
            condition.CheckIfInputString = checkIfInputStringType;

            var conditionInputItem = conditionItem.ConditionInput.TargetItem;
            Hi.UrlRewrite.Entities.ConditionInputType? conditionInputType = null;
            if (conditionInputItem != null)
            {
                switch (conditionInputItem.ID.ToString())
                {
                    case Constants.ConditionInputType_QueryString_ItemId:
                        conditionInputType = Hi.UrlRewrite.Entities.ConditionInputType.QUERY_STRING;
                        break;
                    case Constants.ConditionInputType_HttpHost_ItemId:
                        conditionInputType = Hi.UrlRewrite.Entities.ConditionInputType.HTTP_HOST;
                        break;
                    case Constants.ConditionInputType_Https_ItemId:
                        conditionInputType = Hi.UrlRewrite.Entities.ConditionInputType.HTTPS;
                        break;
                    default:
                        break;
                }
            }
            condition.ConditionInput = conditionInputType;

            return condition;
        }
    }
}

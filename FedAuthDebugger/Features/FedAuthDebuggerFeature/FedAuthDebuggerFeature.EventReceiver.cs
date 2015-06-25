using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace FedAuthDebugger.Features.FedAuthDebuggerFeature
{
    [Guid("475d8464-a31a-4465-8f7d-314c6e909489")]
    public class FedAuthDebuggerFeatureEventReceiver : SPFeatureReceiver
    {
        private SPWebConfigModification CreateWebModificationObject()
        {
            var name = String.Format("add[@name=\"{0}\"]", typeof(FedAuthDebuggerHttpModule).Name);
            var xpath = "/configuration/system.webServer/modules";

            var modification = new SPWebConfigModification(name, xpath);
            modification.Owner = "FedAuthDebugger";
            modification.Sequence = 0;
            modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            modification.Value = String.Format("<add name=\"{0}\" type=\"{1}\" />",
                typeof(FedAuthDebuggerHttpModule).Name, typeof(FedAuthDebuggerHttpModule).AssemblyQualifiedName);
            return modification;
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebConfigModification webConfigModification = CreateWebModificationObject();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                var contentService = SPWebService.ContentService;
                contentService.WebConfigModifications.Add(webConfigModification);
                contentService.Update();
                contentService.ApplyWebConfigModifications();
            });
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebConfigModification webConfigModification = CreateWebModificationObject();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                var contentService = properties.Definition.Farm.Services.GetValue<SPWebService>();

                int numberOfModifications = contentService.WebConfigModifications.Count;
                for (int i = numberOfModifications - 1; i >= 0; i--)
                {
                    var modification = contentService.WebConfigModifications[i];
                    if (modification.Owner.Equals(webConfigModification.Owner))
                    {
                        contentService.WebConfigModifications.Remove(modification);
                    }
                }

                if (numberOfModifications > contentService.WebConfigModifications.Count)
                {
                    contentService.Update();
                    contentService.ApplyWebConfigModifications();
                }
            });
        }
    }
}

using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Crm.Sdk.Messages;

namespace DynService_v3
{
    /// <summary>
    /// Summary description for CRMConnection
    /// </summary>
    public class DynCRMConnection
    {
        private string _user;
        private string _pass;
        private string _domain;
        private string _url;
        private IOrganizationService _service;
        public DynCRMConnection()
        {
            _user = "";
            _pass = "";
            _domain = "";
            _url = "";
            _service = null;
        }

        public DynCRMConnection(string user, string password, string domain, string url)
        {
            _user = user;
            _pass = password;
            _domain = domain;
            _url = url;
        }

        private static bool AcceptAllCertificatePolicy(
            Object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Connect()
        {
            ClientCredentials credentials = new ClientCredentials();

            credentials.Windows.ClientCredential = new System.Net.NetworkCredential(_user, _pass, _domain);

            Uri organizationUri = new Uri(_url);

            Uri homeRealmUri = null;
            OrganizationServiceProxy orgService = new OrganizationServiceProxy(organizationUri, homeRealmUri, credentials, null);
            orgService.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
            _service = (IOrganizationService)orgService;
        }

        public RetrieveMultipleResponse RetrieveMultiple(RetrieveMultipleRequest request)
        {
            return (RetrieveMultipleResponse)_service.Execute(request);
        }

        public Guid Create(Entity e)
        {
            return _service.Create(e);
        }

        public void Update(Entity e)
        {
            _service.Update(e);
        }

        public Entity Retrieve(string EntityName, Guid Id, ColumnSet columnSet)
        {
            return _service.Retrieve(EntityName, Id, columnSet);
        }

        public EntityCollection RetrieveMultiple(QueryExpression query)
        {
            return _service.RetrieveMultiple(query);
        }

        public string GetOptionSetValueLabel(string entityname, string attribute, OptionSetValue option)
        {
            string optionLabel = String.Empty;

            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityname,
                LogicalName = attribute,
                RetrieveAsIfPublished = true
            };

            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)_service.Execute(attributeRequest);
            AttributeMetadata attrMetadata = (AttributeMetadata)attributeResponse.AttributeMetadata;
            PicklistAttributeMetadata picklistMetadata = (PicklistAttributeMetadata)attrMetadata;

            // For every status code value within all of our status codes values
            // (all of the values in the drop down list)
            foreach (OptionMetadata optionMeta in
            picklistMetadata.OptionSet.Options)
            {
                // Check to see if our current value matches
                if (optionMeta.Value == option.Value)
                {
                    // If our numeric value matches, set the string to our status code
                    // label
                    optionLabel = optionMeta.Label.UserLocalizedLabel.Label;
                }
            }

            return optionLabel;
        }

        public SetStateResponse ExecuteStateChange(SetStateRequest request)
        {
            return (SetStateResponse)_service.Execute(request);
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
        }
        //public OptionSetValue FindOptionListItem(string entityName, string optionSetName, string labelToFind)
        //{
        //    return CRMHelper.FindOptionListItem(_service, entityName, optionSetName, labelToFind);
        //}
    }
}
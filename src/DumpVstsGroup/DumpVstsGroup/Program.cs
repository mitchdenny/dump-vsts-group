using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpVstsGroup
{
    public class Program
    {
        private static string GetInput(string inputName, string inputDefault)
        {
            Console.Write($"{inputName} ({inputDefault}): ");
            var input = Console.ReadLine();

            return string.IsNullOrWhiteSpace(input) ? inputDefault : input;
        }

        public static void Main(string[] args)
        {
            var collectionUri = GetInput("Project Collection URI", "http://midenn.visualstudio.com");
            var group = GetInput("Group", "Team Foundation Administrators");

            var collection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("https://midenn.visualstudio.com"));

            var identityService = collection.GetService<IIdentityManagementService>();
            var pcaGroup = identityService.ReadIdentity(IdentitySearchFactor.DisplayName, group, MembershipQuery.Direct, ReadIdentityOptions.ExtendedProperties);
            var pcaMembers = identityService.ReadIdentities(pcaGroup.Members, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties);

            foreach (var pcaMember in pcaMembers)
            {
                var schemaClassName = (string)pcaMember.GetProperty("SchemaClassName");

                if (schemaClassName == "User")
                {
                    var account = pcaMember.GetProperty("Account");
                    Console.WriteLine($"\"{pcaMember.TeamFoundationId}\",\"{pcaMember.DisplayName}\",\"{account}\"");
                }
            }
        }
    }
}

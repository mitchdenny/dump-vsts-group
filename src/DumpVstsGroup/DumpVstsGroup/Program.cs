using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Git.Client;
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
            var collectionUri = GetInput("Project Collection URI", "https://microsoft.visualstudio.com");
            var group = GetInput("Group", "Team Foundation Administrators");
            Console.WriteLine();

            var collection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(collectionUri));

            var identityService = collection.GetService<IIdentityManagementService>();

            var listOfGroupsMatchingInput = identityService.ReadIdentities(IdentitySearchFactor.DisplayName, new[] { group }, MembershipQuery.Direct, ReadIdentityOptions.None);

            foreach (var groupIdentity in listOfGroupsMatchingInput[0])
            {
                Console.WriteLine($"{groupIdentity.GetProperty("SpecialType")} Group: {groupIdentity.DisplayName} ({groupIdentity.TeamFoundationId})");

                var listOfMembersInGroup = identityService.ReadIdentities(groupIdentity.Members, MembershipQuery.None, ReadIdentityOptions.None);

                foreach (var memberIdentity in listOfMembersInGroup)
                {
                    if ((string)memberIdentity.GetProperty("SchemaClassName") == "User")
                    {
                        Console.WriteLine($"\t{memberIdentity.DisplayName} ({memberIdentity.GetProperty("Account")}");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}

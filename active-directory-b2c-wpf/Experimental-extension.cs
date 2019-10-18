using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client
{
    public static class TrustedFrameworkPolicyAppBuilderExtension
    {
        /// <summary>
        /// WithAuthority override enabling Trusted framework policies (for instance for Azure AD B2C)
        /// when building an application
        /// </summary>
        /// <param name="builder">Application builder on which to apply the WithAuthority override</param>
        /// <param name="baseAuthority">Base authority. This is the concatenation of authority host URL, and tenant</param>
        /// <param name="trustedFrameworkPolicy">Policy</param>
        /// <returns>The builder to be chained</returns>
        public static T WithAuthority<T>(this T builder,
                                      string baseAuthority,
                                      string trustedFrameworkPolicy) where T : AbstractApplicationBuilder<T>
        {
            if (string.IsNullOrWhiteSpace(baseAuthority))
            {
                throw new ArgumentException($"{nameof(baseAuthority)} should not be null or only spaces", nameof(baseAuthority));
            }
            if (string.IsNullOrWhiteSpace(trustedFrameworkPolicy))
            {
                throw new ArgumentException($"{nameof(trustedFrameworkPolicy)} should not be null or only spaces", nameof(trustedFrameworkPolicy));
            }

            string b2cAuthority = baseAuthority.EndsWith("/") ? baseAuthority + trustedFrameworkPolicy
                                                              : $"{baseAuthority}/{trustedFrameworkPolicy}";
            return builder.WithB2CAuthority(b2cAuthority);
        }
    }

    public static class TrustedFrameworkPolicyAcquireTokenBuilderExtension
    {
        /// <summary>
        /// WithAuthority override enabling Trusted framework policies (for instance for Azure AD B2C)
        /// when acquiring a token
        /// </summary>
        /// <param name="builder">Application builder on which to apply the WithAuthority override</param>
        /// <param name="baseAuthority">Base authority. This is the concatenation of authority host URL, and tenant</param>
        /// <param name="trustedFrameworkPolicy">Policy</param>
        /// <returns>The builder to be chained</returns>
        /// <returns></returns>
        public static T WithAuthority<T>(this T builder,
                      string baseAuthority,
                      string trustedFrameworkPolicy) where T : AbstractAcquireTokenParameterBuilder<T>
        {
            if (string.IsNullOrWhiteSpace(baseAuthority))
            {
                throw new ArgumentException($"{nameof(baseAuthority)} should not be null or only spaces", nameof(baseAuthority));
            }
            if (string.IsNullOrWhiteSpace(trustedFrameworkPolicy))
            {
                throw new ArgumentException($"{nameof(trustedFrameworkPolicy)} should not be null or only spaces", nameof(trustedFrameworkPolicy));
            }

            string b2cAuthority = baseAuthority.EndsWith("/") ? baseAuthority + trustedFrameworkPolicy
                                                              : $"{baseAuthority}/{trustedFrameworkPolicy}";
            return builder.WithB2CAuthority(b2cAuthority);
        }
    }

    public static class TrustedFrameworkPolicyAppExtension
    {
        /// <summary>
        /// Get the accounts in the cache which fullfil a given trusted framework policy
        /// </summary>
        /// <param name="app">MSAL Application</param>
        /// <param name="trustedFrameworkPolicy">Policy for which to filter the accounts</param>
        /// <returns>Accounts for the given policy</returns>
        public static async Task<IEnumerable<IAccount>> GetAccountsAsync(this IClientApplicationBase app, string trustedFrameworkPolicy)
        {
            if (string.IsNullOrWhiteSpace(trustedFrameworkPolicy))
            {
                throw new ArgumentException($"{nameof(trustedFrameworkPolicy)} should not be null or only spaces", nameof(trustedFrameworkPolicy));
            }

            string lowerCasePolicy = trustedFrameworkPolicy.ToLower();
            IEnumerable<IAccount> accounts = await app.GetAccountsAsync();
            return accounts.Where(account => account.HomeAccountId
                                                    .ObjectId
                                                    .Split('.')[0]
                                                    .EndsWith(lowerCasePolicy));
        }
    }
}

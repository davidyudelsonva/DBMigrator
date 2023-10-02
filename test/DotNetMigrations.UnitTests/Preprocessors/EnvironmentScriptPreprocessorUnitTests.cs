using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetMigrations.ScriptPreprocessors;
using DotNetMigrations.UnitTests.Stubs;
using DotNetMigrations.Core;
using NUnit.Framework;

namespace DotNetMigrations.UnitTests.Preprocessors
{
	[TestFixture]
	public class EnvironmentScriptPreprocessorUnitTests
	{
        private const string ScriptBegin =
			"BEGIN_SETUP: \n SELECT 'Setup' \n";
        private const string ScriptMiddle =
			"END_SETUP:\nBEGIN_TEARDOWN: \n SELECT 'Teardown' \n";
        private const string ScriptEnd =
			"\nEND_TEARDOWN:\n";
        private const string EnvFragment =
			"BEGIN_{0}_ENV: \n SELECT 'Environment {0} {1}' \nEND_{0}_ENV:\n";
        private const string EnvFragmentWithoutTags =
            " \n SELECT 'Environment {0} {1}' \n\n";

        private readonly List<string> _environments = new List<string> {"CentevaDev", "DevAlpha", "DevBeta", "Local", "Test", "iFAMS", "Train", "Preprod", "Production"};

		[Test]
		public void TestEnvironmentReplacement()
		{
			var config = new InMemoryConfigurationManager();
            var script = CreateScript();

            foreach (var currentEnv in _environments)
			{
                string currentEnvFromSettings = currentEnv;
				config.AppSettings[AppSettingKeys.Environment] = currentEnvFromSettings;
				var processor = new EnvironmentScriptPreprocessor(config);

				var processedScript = processor.ModifyScript(script);

                AssertScriptContainsStandardParts(processedScript);
                AssertCurrentEnvironmentSectionsRetained(processedScript, currentEnv, currentEnvFromSettings);
                foreach (var otherEnv in _environments.Where(e => e != currentEnv))
                {
                    AssertOtherEnvironmentSectionsRemoved(processedScript, otherEnv, currentEnvFromSettings);
                }
			}
		}

        [Test]
        public void TestEnvironmentReplacementCaseInsensitive()
        {
            var config = new InMemoryConfigurationManager();
            var script = CreateScript();

            foreach (var currentEnv in _environments)
            {
                string currentEnvFromSettings = currentEnv.ToUpper();
                config.AppSettings[AppSettingKeys.Environment] = currentEnvFromSettings; // Uppercase in settings
                var processor = new EnvironmentScriptPreprocessor(config);

                var processedScript = processor.ModifyScript(script);

                AssertScriptContainsStandardParts(processedScript);
                AssertCurrentEnvironmentSectionsRetained(processedScript, currentEnv, currentEnvFromSettings);
                foreach (var otherEnv in _environments.Where(e => e != currentEnv))
                {
                    AssertOtherEnvironmentSectionsRemoved(processedScript, otherEnv, currentEnvFromSettings);
                }
            }
        }

        private void AssertScriptContainsStandardParts(string processedScript)
        {
            Assert.IsTrue(processedScript.StartsWith(ScriptBegin),"Script is missing standard beginning");
            Assert.IsTrue(processedScript.Contains(ScriptMiddle), "Script it missing standard middle");
            Assert.IsTrue(processedScript.EndsWith(ScriptEnd), "Script is missing standard end");
        }

        private void AssertCurrentEnvironmentSectionsRetained(string processedScript, string environment, string currentEnvironmentFromSettings)
        {
            var errorMessage = $"Environment {environment} fragment was wrongly removed in {currentEnvironmentFromSettings} environment";

            Assert.True(processedScript.Contains(string.Format(EnvFragmentWithoutTags, environment, "Setup")), errorMessage);
            Assert.True(processedScript.Contains(string.Format(EnvFragmentWithoutTags, environment, "Teardown")), errorMessage);
        }

        private void AssertOtherEnvironmentSectionsRemoved(string processedScript, string environment, string currentEnvironmentFromSettings)
        {
            var errorMessage = $"Environment {environment} fragment was not removed in {currentEnvironmentFromSettings} environment";

            Assert.False(processedScript.Contains(string.Format(EnvFragment, environment, "Setup")), errorMessage);
            Assert.False(processedScript.Contains(string.Format(EnvFragment, environment, "Teardown")), errorMessage);
        }

        protected string CreateScript()
        {
            var script = ScriptBegin +
                         CreateEnvironmentFragments("Setup") +
                         ScriptMiddle +
                         CreateEnvironmentFragments("Teardown") +
                         ScriptEnd;
            return script;
        }

		protected string CreateEnvironmentFragments(string section)
		{
			var sb = new StringBuilder();
			foreach (var env in _environments)
			{
				sb.AppendLine(string.Format(EnvFragment, env, section));
			}
			return sb.ToString();
		}


	}
}

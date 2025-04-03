using System;
using System.Threading.Tasks;

namespace MCPnet.Client.Examples
{
    /// <summary>
    /// Runner class to demonstrate all MCPnet example scenarios
    /// </summary>
    public static class ExampleRunner
    {
        /// <summary>
        /// Run all examples to demonstrate MCPnet capabilities
        /// </summary>
        public static async Task RunAllExamplesAsync()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("        MCPnet Library Demonstration Suite       ");
            Console.WriteLine("=================================================");
            Console.WriteLine();
            
            await RunBasicExamplesAsync();
            await RunIndustryExamplesAsync();
            await RunEnterpriseExamplesAsync();
        }
        
        /// <summary>
        /// Run the basic examples showing core functionality
        /// </summary>
        public static async Task RunBasicExamplesAsync()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("           Basic MCPnet Functionality            ");
            Console.WriteLine("=================================================");
            Console.WriteLine();
            
            try
            {
                await ExampleUsage.RunExampleAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running basic examples: {ex.Message}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Basic examples completed.");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Run the industry-specific examples
        /// </summary>
        public static async Task RunIndustryExamplesAsync()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("        Industry-Specific MCPnet Examples        ");
            Console.WriteLine("=================================================");
            Console.WriteLine();
            
            try
            {
                Console.WriteLine("[1] Healthcare Example");
                Console.WriteLine("-----------------------");
                await IndustrySpecificExamples.HealthcareExampleAsync();
                Console.WriteLine();
                
                Console.WriteLine("[2] Finance Example");
                Console.WriteLine("-----------------------");
                await IndustrySpecificExamples.FinanceExampleAsync();
                Console.WriteLine();
                
                Console.WriteLine("[3] Content Creation Example");
                Console.WriteLine("-----------------------");
                await IndustrySpecificExamples.ContentCreationExampleAsync();
                Console.WriteLine();
                
                Console.WriteLine("[4] Customer Service Example");
                Console.WriteLine("-----------------------");
                await IndustrySpecificExamples.CustomerServiceExampleAsync();
                Console.WriteLine();
                
                Console.WriteLine("[5] Scientific Research Example");
                Console.WriteLine("-----------------------");
                await IndustrySpecificExamples.ScientificResearchExampleAsync();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running industry examples: {ex.Message}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Industry examples completed.");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Run the enterprise integration examples
        /// </summary>
        public static async Task RunEnterpriseExamplesAsync()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("      Enterprise Integration MCPnet Examples     ");
            Console.WriteLine("=================================================");
            Console.WriteLine();
            
            try
            {
                Console.WriteLine("[1] Document Processing Workflow");
                Console.WriteLine("-----------------------");
                await EnterpriseIntegrationExamples.DocumentProcessingWorkflowAsync();
                Console.WriteLine();
                
                Console.WriteLine("[2] IoT Data Analysis");
                Console.WriteLine("-----------------------");
                await EnterpriseIntegrationExamples.IoTDataAnalysisAsync();
                Console.WriteLine();
                
                Console.WriteLine("[3] SAP Supply Chain Integration");
                Console.WriteLine("-----------------------");
                await EnterpriseIntegrationExamples.SapSupplyChainIntegrationAsync();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running enterprise examples: {ex.Message}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Enterprise examples completed.");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Entry point for console applications
        /// </summary>
        public static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                // Run specific examples based on arguments
                foreach (var arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "basic":
                            await RunBasicExamplesAsync();
                            break;
                        case "industry":
                            await RunIndustryExamplesAsync();
                            break;
                        case "enterprise":
                            await RunEnterpriseExamplesAsync();
                            break;
                        case "healthcare":
                            await IndustrySpecificExamples.HealthcareExampleAsync();
                            break;
                        case "finance":
                            await IndustrySpecificExamples.FinanceExampleAsync();
                            break;
                        case "content":
                            await IndustrySpecificExamples.ContentCreationExampleAsync();
                            break;
                        case "customer":
                            await IndustrySpecificExamples.CustomerServiceExampleAsync();
                            break;
                        case "science":
                            await IndustrySpecificExamples.ScientificResearchExampleAsync();
                            break;
                        case "document":
                            await EnterpriseIntegrationExamples.DocumentProcessingWorkflowAsync();
                            break;
                        case "iot":
                            await EnterpriseIntegrationExamples.IoTDataAnalysisAsync();
                            break;
                        case "sap":
                            await EnterpriseIntegrationExamples.SapSupplyChainIntegrationAsync();
                            break;
                        default:
                            Console.WriteLine($"Unknown example: {arg}");
                            break;
                    }
                }
            }
            else
            {
                // Run all examples
                await RunAllExamplesAsync();
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
} 
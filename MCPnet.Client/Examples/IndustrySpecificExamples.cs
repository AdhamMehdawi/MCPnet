using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MCPnet.Client.Extensions;
using MCPnet.Core.Interfaces;
using MCPnet.Core.Models;

namespace MCPnet.Client.Examples
{
    /// <summary>
    /// Industry-specific examples of how to use MCPnet in real-world applications
    /// </summary>
    public static class IndustrySpecificExamples
    {
        /// <summary>
        /// Example of using MCPnet in a healthcare application
        /// </summary>
        public static async Task HealthcareExampleAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            // Healthcare example: Find medical imaging analysis capabilities
            var medicalServers = await mcpRegistry.SearchServersAsync(
                query: "medical imaging analysis radiology",
                minRating: 4.5);
            
            Console.WriteLine($"Found {medicalServers.Count} medical imaging servers");
            
            if (medicalServers.Count > 0)
            {
                // Example: Process a CT scan for diagnosis assistance
                var ctScanServer = medicalServers[0];
                var capabilities = await mcpClient.DiscoverCapabilitiesAsync(ctScanServer.Url);
                
                // Find a capability for lung nodule detection
                var lungCapability = capabilities.Find(c => 
                    c.Name.Contains("lung", StringComparison.OrdinalIgnoreCase) && 
                    c.Name.Contains("nodule", StringComparison.OrdinalIgnoreCase));
                
                if (lungCapability != null)
                {
                    Console.WriteLine($"Found lung nodule detection capability: {lungCapability.Name}");
                    
                    // In a real application, this would be a base64 encoded image or a URL to a DICOM file
                    var ctScanData = "base64_encoded_ct_scan_image_data";
                    
                    // Prepare the request with the CT scan image
                    var request = new McpRequest
                    {
                        CapabilityId = lungCapability.Id,
                        Input = new Dictionary<string, object>
                        {
                            { "image_data", ctScanData },
                            { "patient_age", 65 },
                            { "patient_sex", "male" },
                            { "scan_type", "chest_ct" }
                        }
                    };
                    
                    // Invoke the capability
                    var response = await mcpClient.InvokeCapabilityAsync(ctScanServer.Url, request);
                    
                    if (response.Success)
                    {
                        Console.WriteLine("Lung nodule detection results:");
                        var findings = response.Output["findings"] as Dictionary<string, object>;
                        Console.WriteLine($"  Nodules detected: {findings["nodule_count"]}");
                        Console.WriteLine($"  Malignancy risk: {findings["malignancy_risk"]}");
                        Console.WriteLine($"  Recommendation: {findings["recommendation"]}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Example of using MCPnet in a finance application
        /// </summary>
        public static async Task FinanceExampleAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            // Finance example: Create a custom financial analysis toolkit
            Console.WriteLine("Building a custom financial analysis toolkit...");
            
            // Find sentiment analysis for financial news
            var sentimentCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "financial news sentiment analysis", 
                minRating: 4.0);
                
            // Find stock prediction capabilities
            var predictionCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "stock price prediction machine learning", 
                minRating: 4.2);
                
            // Find fraud detection capabilities
            var fraudCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "financial transaction fraud detection", 
                minRating: 4.7);
            
            // Create a capability remix with the best tools
            var capabilitiesToRemix = new List<(string ServerId, string CapabilityId)>();
            
            if (sentimentCapabilities.Count > 0)
                capabilitiesToRemix.Add((sentimentCapabilities[0].Server.Id, sentimentCapabilities[0].Capability.Id));
                
            if (predictionCapabilities.Count > 0)
                capabilitiesToRemix.Add((predictionCapabilities[0].Server.Id, predictionCapabilities[0].Capability.Id));
                
            if (fraudCapabilities.Count > 0)
                capabilitiesToRemix.Add((fraudCapabilities[0].Server.Id, fraudCapabilities[0].Capability.Id));
            
            if (capabilitiesToRemix.Count > 0)
            {
                var financeToolkit = await mcpRegistry.CreateRemixAsync(
                    name: "Advanced Financial Analysis Toolkit",
                    description: "A comprehensive toolkit for financial analysis, prediction and fraud detection",
                    capabilities: capabilitiesToRemix);
                
                Console.WriteLine($"Created financial toolkit: {financeToolkit.Name}");
                Console.WriteLine($"Capabilities included: {financeToolkit.Capabilities.Count}");
                
                // Example: Analyze a financial news article for market sentiment
                if (financeToolkit.Capabilities.Count > 0)
                {
                    var sentimentCapability = financeToolkit.Capabilities.Find(c => 
                        c.Name.Contains("sentiment", StringComparison.OrdinalIgnoreCase));
                    
                    if (sentimentCapability != null)
                    {
                        var request = new McpRequest
                        {
                            CapabilityId = sentimentCapability.Id,
                            Input = new Dictionary<string, object>
                            {
                                { "text", "XYZ Corp reported quarterly earnings exceeding analyst expectations by 15%. The company also announced an expansion into emerging markets." },
                                { "companies", new[] { "XYZ Corp" } },
                                { "indicators", new[] { "earnings", "expansion", "markets" } }
                            }
                        };
                        
                        var response = await mcpClient.InvokeCapabilityAsync(financeToolkit.Url, request);
                        
                        if (response.Success)
                        {
                            Console.WriteLine("\nSentiment analysis results:");
                            var sentiment = response.Output["sentiment"] as Dictionary<string, object>;
                            Console.WriteLine($"  Overall sentiment: {sentiment["overall"]}");
                            Console.WriteLine($"  Market impact prediction: {sentiment["market_impact"]}");
                            Console.WriteLine($"  Confidence score: {sentiment["confidence"]}");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Example of using MCPnet in a content creation application
        /// </summary>
        public static async Task ContentCreationExampleAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            Console.WriteLine("Setting up a multimedia content creation workflow...");
            
            // Find text-to-image generation capabilities
            var imageGenCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "text to image generation high quality", 
                minRating: 4.5);
                
            // Find audio narration capabilities
            var audioCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "text to speech natural voice", 
                minRating: 4.3);
                
            // Find video editing capabilities
            var videoCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "automated video editing", 
                minRating: 4.0);
            
            // Example: Create a short promotional video from a text prompt
            if (imageGenCapabilities.Count > 0 && audioCapabilities.Count > 0)
            {
                Console.WriteLine("\nGenerating promotional content from text description...");
                
                // Step 1: Generate images based on product description
                var imageCapability = imageGenCapabilities[0].Capability;
                var imageServer = imageGenCapabilities[0].Server;
                
                var imageRequest = new McpRequest
                {
                    CapabilityId = imageCapability.Id,
                    Input = new Dictionary<string, object>
                    {
                        { "prompt", "A sleek modern smartphone with an edge-to-edge display reflecting a mountain landscape" },
                        { "style", "photorealistic product photography" },
                        { "aspect_ratio", "16:9" },
                        { "count", 3 }
                    }
                };
                
                var imageResponse = await mcpClient.InvokeCapabilityAsync(imageServer.Url, imageRequest);
                
                if (imageResponse.Success)
                {
                    Console.WriteLine("Generated product images successfully");
                    var imageUrls = imageResponse.Output["image_urls"] as IEnumerable<string>;
                    
                    // Step 2: Generate narration audio for the promotional video
                    if (audioCapabilities.Count > 0)
                    {
                        var audioCapability = audioCapabilities[0].Capability;
                        var audioServer = audioCapabilities[0].Server;
                        
                        var audioRequest = new McpRequest
                        {
                            CapabilityId = audioCapability.Id,
                            Input = new Dictionary<string, object>
                            {
                                { "text", "Introducing our latest smartphone. With an edge-to-edge display and revolutionary camera system, you'll capture memories with unprecedented clarity. Experience the future of mobile technology today." },
                                { "voice", "professional_male_en_us" },
                                { "speed", 1.0 },
                                { "emphasis", new[] { "revolutionary", "unprecedented", "future" } }
                            }
                        };
                        
                        var audioResponse = await mcpClient.InvokeCapabilityAsync(audioServer.Url, audioRequest);
                        
                        if (audioResponse.Success)
                        {
                            Console.WriteLine("Generated narration audio successfully");
                            var audioUrl = audioResponse.Output["audio_url"] as string;
                            
                            // Step 3: If we have video capabilities, combine images and audio into a video
                            if (videoCapabilities.Count > 0)
                            {
                                var videoCapability = videoCapabilities[0].Capability;
                                var videoServer = videoCapabilities[0].Server;
                                
                                var videoRequest = new McpRequest
                                {
                                    CapabilityId = videoCapability.Id,
                                    Input = new Dictionary<string, object>
                                    {
                                        { "images", imageUrls },
                                        { "audio_track", audioUrl },
                                        { "duration", 30 },
                                        { "transitions", "smooth_fade" },
                                        { "text_overlay", "NEW PRODUCT LAUNCH | AVAILABLE NOW" }
                                    }
                                };
                                
                                var videoResponse = await mcpClient.InvokeCapabilityAsync(videoServer.Url, videoRequest);
                                
                                if (videoResponse.Success)
                                {
                                    Console.WriteLine("Created promotional video successfully");
                                    var videoUrl = videoResponse.Output["video_url"] as string;
                                    Console.WriteLine($"Video URL: {videoUrl}");
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Example of using MCPnet in a customer service application
        /// </summary>
        public static async Task CustomerServiceExampleAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            Console.WriteLine("Setting up an intelligent customer service system...");
            
            // Find language understanding capabilities
            var languageCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "natural language understanding intent detection", 
                minRating: 4.6);
                
            // Find knowledge retrieval capabilities
            var knowledgeCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "knowledge base retrieval question answering", 
                minRating: 4.4);
                
            // Find sentiment analysis capabilities
            var sentimentCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "customer feedback sentiment analysis", 
                minRating: 4.2);
            
            // Example: Process a customer inquiry
            if (languageCapabilities.Count > 0 && knowledgeCapabilities.Count > 0)
            {
                Console.WriteLine("\nProcessing customer inquiry...");
                
                // Step 1: Analyze the customer's message to understand intent
                var languageCapability = languageCapabilities[0].Capability;
                var languageServer = languageCapabilities[0].Server;
                
                var customerMessage = "I ordered your premium product last week, but I haven't received a shipping confirmation yet. My order number is ORD-12345. Can you help me find out when it will arrive?";
                
                var intentRequest = new McpRequest
                {
                    CapabilityId = languageCapability.Id,
                    Input = new Dictionary<string, object>
                    {
                        { "text", customerMessage },
                        { "possible_intents", new[] { 
                            "order_status", "cancel_order", "return_item", 
                            "product_question", "shipping_info", "complaint" 
                        }}
                    }
                };
                
                var intentResponse = await mcpClient.InvokeCapabilityAsync(languageServer.Url, intentRequest);
                
                if (intentResponse.Success)
                {
                    Console.WriteLine("Analyzed customer intent successfully");
                    var intent = intentResponse.Output["detected_intent"] as string;
                    var entities = intentResponse.Output["entities"] as Dictionary<string, object>;
                    
                    Console.WriteLine($"Customer intent: {intent}");
                    Console.WriteLine($"Order number: {entities["order_number"]}");
                    
                    // Step 2: Retrieve relevant information from knowledge base
                    if (knowledgeCapabilities.Count > 0)
                    {
                        var knowledgeCapability = knowledgeCapabilities[0].Capability;
                        var knowledgeServer = knowledgeCapabilities[0].Server;
                        
                        var knowledgeRequest = new McpRequest
                        {
                            CapabilityId = knowledgeCapability.Id,
                            Input = new Dictionary<string, object>
                            {
                                { "query", $"order status {entities["order_number"]}" },
                                { "context", new Dictionary<string, object> {
                                    { "customer_id", "CUST-67890" },
                                    { "order_number", entities["order_number"] },
                                    { "product_category", "premium" }
                                }}
                            }
                        };
                        
                        var knowledgeResponse = await mcpClient.InvokeCapabilityAsync(knowledgeServer.Url, knowledgeRequest);
                        
                        if (knowledgeResponse.Success)
                        {
                            Console.WriteLine("Retrieved order information successfully");
                            var orderInfo = knowledgeResponse.Output["order_info"] as Dictionary<string, object>;
                            
                            Console.WriteLine($"Order status: {orderInfo["status"]}");
                            Console.WriteLine($"Expected delivery: {orderInfo["expected_delivery"]}");
                            Console.WriteLine($"Shipping carrier: {orderInfo["carrier"]}");
                            Console.WriteLine($"Tracking number: {orderInfo["tracking_number"]}");
                            
                            // Generate a response to the customer
                            Console.WriteLine("\nAutomated response to customer:");
                            Console.WriteLine($"Thank you for contacting us about your order {entities["order_number"]}. " +
                                $"Your order is currently {orderInfo["status"]} and is expected to be delivered by {orderInfo["expected_delivery"]}. " +
                                $"You can track your package with {orderInfo["carrier"]} using tracking number {orderInfo["tracking_number"]}.");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Example of using MCPnet for scientific research
        /// </summary>
        public static async Task ScientificResearchExampleAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            Console.WriteLine("Setting up a scientific data analysis pipeline...");
            
            // Find protein structure prediction capabilities
            var proteinCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "protein structure prediction", 
                minRating: 4.7);
                
            // Find molecular docking simulation capabilities
            var dockingCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "molecular docking simulation", 
                minRating: 4.5);
                
            // Find research paper analysis capabilities
            var paperCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "scientific literature analysis research papers", 
                minRating: 4.3);
            
            // Example: Run a drug discovery workflow
            if (proteinCapabilities.Count > 0 && dockingCapabilities.Count > 0)
            {
                Console.WriteLine("\nRunning drug discovery simulation workflow...");
                
                // Step 1: Predict the structure of a target protein
                var proteinCapability = proteinCapabilities[0].Capability;
                var proteinServer = proteinCapabilities[0].Server;
                
                // In a real scenario, this would be the amino acid sequence of the protein
                var proteinSequence = "MKTVRQERLKSIVRILERSKEPVSGAQLAEELSVSRQVIVQDIAYLRSLGYNIVATPRGYVLAGG";
                
                var proteinRequest = new McpRequest
                {
                    CapabilityId = proteinCapability.Id,
                    Input = new Dictionary<string, object>
                    {
                        { "sequence", proteinSequence },
                        { "model_quality", "high" },
                        { "output_format", "pdb" }
                    }
                };
                
                var proteinResponse = await mcpClient.InvokeCapabilityAsync(proteinServer.Url, proteinRequest);
                
                if (proteinResponse.Success)
                {
                    Console.WriteLine("Protein structure prediction successful");
                    var structureUrl = proteinResponse.Output["structure_url"] as string;
                    var confidenceScore = proteinResponse.Output["confidence_score"] as double?;
                    
                    Console.WriteLine($"Structure URL: {structureUrl}");
                    Console.WriteLine($"Confidence score: {confidenceScore}");
                    
                    // Step 2: Perform molecular docking with candidate compounds
                    if (dockingCapabilities.Count > 0)
                    {
                        var dockingCapability = dockingCapabilities[0].Capability;
                        var dockingServer = dockingCapabilities[0].Server;
                        
                        // In a real scenario, these would be molecular descriptors of candidate compounds
                        var candidateCompounds = new[] {
                            new Dictionary<string, object> { 
                                { "id", "COMP-001" }, 
                                { "smiles", "CC(=O)OC1=CC=CC=C1C(=O)O" } 
                            },
                            new Dictionary<string, object> { 
                                { "id", "COMP-002" }, 
                                { "smiles", "COC1=CC=C(CC2=CN=C(N)N=C2N)C=C1" } 
                            }
                        };
                        
                        var dockingRequest = new McpRequest
                        {
                            CapabilityId = dockingCapability.Id,
                            Input = new Dictionary<string, object>
                            {
                                { "protein_structure_url", structureUrl },
                                { "candidate_compounds", candidateCompounds },
                                { "binding_site", new Dictionary<string, object> {
                                    { "center_x", 23.5 },
                                    { "center_y", 14.2 },
                                    { "center_z", -8.7 },
                                    { "size_x", 10.0 },
                                    { "size_y", 10.0 },
                                    { "size_z", 10.0 }
                                }}
                            }
                        };
                        
                        var dockingResponse = await mcpClient.InvokeCapabilityAsync(dockingServer.Url, dockingRequest);
                        
                        if (dockingResponse.Success)
                        {
                            Console.WriteLine("\nMolecular docking simulation successful");
                            var results = dockingResponse.Output["docking_results"] as IEnumerable<Dictionary<string, object>>;
                            
                            Console.WriteLine("Docking results:");
                            foreach (var result in results)
                            {
                                Console.WriteLine($"  Compound: {result["compound_id"]}");
                                Console.WriteLine($"  Binding affinity: {result["binding_affinity"]} kcal/mol");
                                Console.WriteLine($"  Interactions: {string.Join(", ", result["interactions"] as IEnumerable<string>)}");
                                Console.WriteLine();
                            }
                            
                            // Step 3: Research relevant literature if we have a paper analysis capability
                            if (paperCapabilities.Count > 0)
                            {
                                var paperCapability = paperCapabilities[0].Capability;
                                var paperServer = paperCapabilities[0].Server;
                                
                                var paperRequest = new McpRequest
                                {
                                    CapabilityId = paperCapability.Id,
                                    Input = new Dictionary<string, object>
                                    {
                                        { "query", "drug discovery molecular docking protein inhibitors" },
                                        { "context", new Dictionary<string, object> {
                                            { "protein_target", proteinSequence.Substring(0, 20) + "..." },
                                            { "binding_site", "active site inhibition" },
                                            { "therapeutic_area", "antiviral" }
                                        }},
                                        { "max_results", 5 },
                                        { "time_range", "last 2 years" }
                                    }
                                };
                                
                                var paperResponse = await mcpClient.InvokeCapabilityAsync(paperServer.Url, paperRequest);
                                
                                if (paperResponse.Success)
                                {
                                    Console.WriteLine("Research paper analysis successful");
                                    var papers = paperResponse.Output["papers"] as IEnumerable<Dictionary<string, object>>;
                                    
                                    Console.WriteLine("\nRelevant research papers:");
                                    foreach (var paper in papers)
                                    {
                                        Console.WriteLine($"  Title: {paper["title"]}");
                                        Console.WriteLine($"  Authors: {paper["authors"]}");
                                        Console.WriteLine($"  Journal: {paper["journal"]}, {paper["year"]}");
                                        Console.WriteLine($"  Relevance score: {paper["relevance_score"]}");
                                        Console.WriteLine($"  Key findings: {paper["key_findings"]}");
                                        Console.WriteLine();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
} 
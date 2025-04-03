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
    /// Examples showing how MCPnet can be integrated with enterprise systems and workflows
    /// </summary>
    public static class EnterpriseIntegrationExamples
    {
        /// <summary>
        /// Example of integrating MCPnet with a document processing workflow
        /// </summary>
        public static async Task DocumentProcessingWorkflowAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            Console.WriteLine("Setting up an enterprise document processing workflow...");
            
            // Find document OCR capabilities
            var ocrCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "document OCR extraction high accuracy", 
                minRating: 4.6);
                
            // Find document classification capabilities
            var classificationCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "document classification categorization", 
                minRating: 4.4);
                
            // Find data extraction capabilities
            var extractionCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "structured data extraction from documents", 
                minRating: 4.5);
            
            // Integration example: Process an invoice through multiple stages
            if (ocrCapabilities.Count > 0 && classificationCapabilities.Count > 0 && extractionCapabilities.Count > 0)
            {
                Console.WriteLine("\nProcessing invoice document...");
                
                // In a real scenario, this would be the document file or URL
                var invoiceDocument = "https://example.com/documents/invoice-12345.pdf";
                
                // Step 1: Convert document to text using OCR
                var ocrCapability = ocrCapabilities[0].Capability;
                var ocrServer = ocrCapabilities[0].Server;
                
                var ocrRequest = new McpRequest
                {
                    CapabilityId = ocrCapability.Id,
                    Input = new Dictionary<string, object>
                    {
                        { "document_url", invoiceDocument },
                        { "output_format", "text" },
                        { "language", "en" },
                        { "preserve_layout", true }
                    }
                };
                
                var ocrResponse = await mcpClient.InvokeCapabilityAsync(ocrServer.Url, ocrRequest);
                
                if (ocrResponse.Success)
                {
                    Console.WriteLine("OCR processing successful");
                    var textContent = ocrResponse.Output["text"] as string;
                    var confidence = ocrResponse.Output["confidence"] as double?;
                    
                    Console.WriteLine($"OCR confidence: {confidence}");
                    
                    // Step 2: Classify the document type
                    var classificationCapability = classificationCapabilities[0].Capability;
                    var classificationServer = classificationCapabilities[0].Server;
                    
                    var classificationRequest = new McpRequest
                    {
                        CapabilityId = classificationCapability.Id,
                        Input = new Dictionary<string, object>
                        {
                            { "text", textContent },
                            { "possible_types", new[] { 
                                "invoice", "purchase_order", "receipt", 
                                "contract", "shipping_notice", "statement" 
                            }}
                        }
                    };
                    
                    var classificationResponse = await mcpClient.InvokeCapabilityAsync(classificationServer.Url, classificationRequest);
                    
                    if (classificationResponse.Success)
                    {
                        var documentType = classificationResponse.Output["document_type"] as string;
                        var typeConfidence = classificationResponse.Output["confidence"] as double?;
                        
                        Console.WriteLine($"Document classified as: {documentType} (confidence: {typeConfidence})");
                        
                        // Step 3: Extract structured data based on document type
                        if (documentType == "invoice")
                        {
                            var extractionCapability = extractionCapabilities[0].Capability;
                            var extractionServer = extractionCapabilities[0].Server;
                            
                            var extractionRequest = new McpRequest
                            {
                                CapabilityId = extractionCapability.Id,
                                Input = new Dictionary<string, object>
                                {
                                    { "text", textContent },
                                    { "document_type", documentType },
                                    { "fields", new[] { 
                                        "invoice_number", "date", "due_date", "total_amount",
                                        "tax_amount", "vendor_name", "vendor_address",
                                        "line_items", "payment_terms"
                                    }}
                                }
                            };
                            
                            var extractionResponse = await mcpClient.InvokeCapabilityAsync(extractionServer.Url, extractionRequest);
                            
                            if (extractionResponse.Success)
                            {
                                Console.WriteLine("Data extraction successful");
                                var extractedData = extractionResponse.Output["extracted_fields"] as Dictionary<string, object>;
                                
                                Console.WriteLine("\nExtracted invoice data:");
                                Console.WriteLine($"Invoice Number: {extractedData["invoice_number"]}");
                                Console.WriteLine($"Date: {extractedData["date"]}");
                                Console.WriteLine($"Due Date: {extractedData["due_date"]}");
                                Console.WriteLine($"Total Amount: {extractedData["total_amount"]}");
                                Console.WriteLine($"Vendor: {extractedData["vendor_name"]}");
                                
                                // Simulate sending this data to an ERP system
                                Console.WriteLine("\nSending extracted data to ERP system...");
                                Console.WriteLine("Creating payment record in accounts payable...");
                                Console.WriteLine("Updating vendor record with invoice information...");
                                Console.WriteLine("Integration complete - invoice queued for approval");
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Example of using MCPnet for IoT data analysis
        /// </summary>
        public static async Task IoTDataAnalysisAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            Console.WriteLine("Setting up an IoT data analysis pipeline...");
            
            // Find anomaly detection capabilities
            var anomalyCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "time series anomaly detection IoT", 
                minRating: 4.5);
                
            // Find predictive maintenance capabilities
            var maintenanceCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "predictive maintenance equipment failure", 
                minRating: 4.7);
                
            // Find sensor data visualization capabilities
            var visualizationCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "IoT sensor data visualization dashboard", 
                minRating: 4.3);
            
            // Integration example: Analyze factory equipment sensor data
            if (anomalyCapabilities.Count > 0 && maintenanceCapabilities.Count > 0)
            {
                Console.WriteLine("\nProcessing manufacturing equipment sensor data...");
                
                // In a real scenario, this would be time series data from sensors
                var sensorData = new Dictionary<string, object>
                {
                    { "equipment_id", "PUMP-123" },
                    { "timestamp_start", "2025-04-01T00:00:00Z" },
                    { "timestamp_end", "2025-04-03T23:59:59Z" },
                    { "readings", new[] {
                        new Dictionary<string, object> {
                            { "sensor_id", "temp-1" },
                            { "name", "Temperature" },
                            { "unit", "Celsius" },
                            { "values", new[] { 85.2, 85.4, 85.3, 85.5, 86.1, 86.8, 87.3, 88.5, 90.2, 92.1, 93.5, 94.8 } },
                            { "timestamps", new[] { 
                                "2025-04-01T00:00:00Z", "2025-04-01T02:00:00Z", "2025-04-01T04:00:00Z",
                                "2025-04-01T06:00:00Z", "2025-04-01T08:00:00Z", "2025-04-01T10:00:00Z",
                                "2025-04-01T12:00:00Z", "2025-04-01T14:00:00Z", "2025-04-01T16:00:00Z",
                                "2025-04-01T18:00:00Z", "2025-04-01T20:00:00Z", "2025-04-01T22:00:00Z"
                            }}
                        },
                        new Dictionary<string, object> {
                            { "sensor_id", "pressure-1" },
                            { "name", "Pressure" },
                            { "unit", "PSI" },
                            { "values", new[] { 45.2, 45.3, 45.1, 45.3, 45.0, 44.8, 44.5, 44.2, 43.8, 43.1, 42.5, 42.0 } },
                            { "timestamps", new[] { 
                                "2025-04-01T00:00:00Z", "2025-04-01T02:00:00Z", "2025-04-01T04:00:00Z",
                                "2025-04-01T06:00:00Z", "2025-04-01T08:00:00Z", "2025-04-01T10:00:00Z",
                                "2025-04-01T12:00:00Z", "2025-04-01T14:00:00Z", "2025-04-01T16:00:00Z",
                                "2025-04-01T18:00:00Z", "2025-04-01T20:00:00Z", "2025-04-01T22:00:00Z"
                            }}
                        },
                        new Dictionary<string, object> {
                            { "sensor_id", "vibration-1" },
                            { "name", "Vibration" },
                            { "unit", "mm/s" },
                            { "values", new[] { 2.1, 2.0, 2.2, 2.3, 2.5, 2.8, 3.2, 3.8, 4.5, 5.2, 5.8, 6.5 } },
                            { "timestamps", new[] { 
                                "2025-04-01T00:00:00Z", "2025-04-01T02:00:00Z", "2025-04-01T04:00:00Z",
                                "2025-04-01T06:00:00Z", "2025-04-01T08:00:00Z", "2025-04-01T10:00:00Z",
                                "2025-04-01T12:00:00Z", "2025-04-01T14:00:00Z", "2025-04-01T16:00:00Z",
                                "2025-04-01T18:00:00Z", "2025-04-01T20:00:00Z", "2025-04-01T22:00:00Z"
                            }}
                        }
                    }}
                };
                
                // Step 1: Detect anomalies in sensor data
                var anomalyCapability = anomalyCapabilities[0].Capability;
                var anomalyServer = anomalyCapabilities[0].Server;
                
                var anomalyRequest = new McpRequest
                {
                    CapabilityId = anomalyCapability.Id,
                    Input = new Dictionary<string, object>
                    {
                        { "sensor_data", sensorData },
                        { "sensitivity", "medium" },
                        { "detection_method", "isolation_forest" }
                    }
                };
                
                var anomalyResponse = await mcpClient.InvokeCapabilityAsync(anomalyServer.Url, anomalyRequest);
                
                if (anomalyResponse.Success)
                {
                    Console.WriteLine("Anomaly detection successful");
                    var anomalies = anomalyResponse.Output["anomalies"] as IEnumerable<Dictionary<string, object>>;
                    
                    Console.WriteLine("\nDetected anomalies:");
                    foreach (var anomaly in anomalies)
                    {
                        Console.WriteLine($"Sensor: {anomaly["sensor_id"]}");
                        Console.WriteLine($"Timestamp: {anomaly["timestamp"]}");
                        Console.WriteLine($"Value: {anomaly["value"]}");
                        Console.WriteLine($"Severity: {anomaly["severity"]}");
                        Console.WriteLine();
                    }
                    
                    // Step 2: Predict maintenance needs
                    if (maintenanceCapabilities.Count > 0)
                    {
                        var maintenanceCapability = maintenanceCapabilities[0].Capability;
                        var maintenanceServer = maintenanceCapabilities[0].Server;
                        
                        var maintenanceRequest = new McpRequest
                        {
                            CapabilityId = maintenanceCapability.Id,
                            Input = new Dictionary<string, object>
                            {
                                { "equipment_id", sensorData["equipment_id"] },
                                { "sensor_data", sensorData },
                                { "anomalies", anomalies },
                                { "equipment_type", "centrifugal_pump" },
                                { "maintenance_history", new[] {
                                    new Dictionary<string, object> {
                                        { "date", "2024-12-15" },
                                        { "type", "preventive" },
                                        { "components_replaced", new[] { "bearing", "seal" } }
                                    },
                                    new Dictionary<string, object> {
                                        { "date", "2024-09-03" },
                                        { "type", "corrective" },
                                        { "components_replaced", new[] { "impeller" } }
                                    }
                                }}
                            }
                        };
                        
                        var maintenanceResponse = await mcpClient.InvokeCapabilityAsync(maintenanceServer.Url, maintenanceRequest);
                        
                        if (maintenanceResponse.Success)
                        {
                            Console.WriteLine("Predictive maintenance analysis successful");
                            var prediction = maintenanceResponse.Output["prediction"] as Dictionary<string, object>;
                            
                            Console.WriteLine("\nMaintenance prediction:");
                            Console.WriteLine($"Failure probability: {prediction["failure_probability"]}");
                            Console.WriteLine($"Estimated time to failure: {prediction["time_to_failure"]} days");
                            Console.WriteLine($"Recommended action: {prediction["recommended_action"]}");
                            
                            var failingComponents = prediction["failing_components"] as IEnumerable<Dictionary<string, object>>;
                            Console.WriteLine("\nPotential failing components:");
                            foreach (var component in failingComponents)
                            {
                                Console.WriteLine($"  Component: {component["name"]}");
                                Console.WriteLine($"  Confidence: {component["confidence"]}");
                                Console.WriteLine($"  Replacement part #: {component["part_number"]}");
                                Console.WriteLine();
                            }
                            
                            // Simulate integration with maintenance management system
                            Console.WriteLine("\nIntegrating with enterprise systems:");
                            Console.WriteLine("Creating work order in CMMS...");
                            Console.WriteLine("Checking spare parts inventory...");
                            Console.WriteLine("Scheduling maintenance technician...");
                            Console.WriteLine("Sending alerts to plant manager dashboard...");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Example of using MCPnet with SAP integration for supply chain management
        /// </summary>
        public static async Task SapSupplyChainIntegrationAsync()
        {
            // Set up the MCPnet services
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            Console.WriteLine("Setting up supply chain optimization with SAP integration...");
            
            // Find demand forecasting capabilities
            var forecastCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "demand forecasting supply chain", 
                minRating: 4.6);
                
            // Find inventory optimization capabilities
            var inventoryCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "inventory optimization stock levels", 
                minRating: 4.5);
                
            // Find supply chain risk analysis capabilities
            var riskCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "supply chain risk analysis disruption", 
                minRating: 4.4);
            
            // Integration example: Optimize inventory and forecast demand
            if (forecastCapabilities.Count > 0 && inventoryCapabilities.Count > 0)
            {
                Console.WriteLine("\nOptimizing inventory for product category: Electronics");
                
                // In a real scenario, this would be data from SAP or other ERP systems
                var historicalSalesData = new Dictionary<string, object>
                {
                    { "product_category", "Electronics" },
                    { "time_period", "2024-01-01 to 2025-03-31" },
                    { "products", new[] {
                        new Dictionary<string, object> {
                            { "product_id", "PROD-10001" },
                            { "name", "4K Smart TV 55-inch" },
                            { "monthly_sales", new[] { 142, 156, 134, 128, 165, 187, 210, 198, 175, 168, 180, 210, 231, 245, 228 } }
                        },
                        new Dictionary<string, object> {
                            { "product_id", "PROD-10002" },
                            { "name", "Wireless Headphones" },
                            { "monthly_sales", new[] { 312, 345, 367, 389, 412, 435, 465, 482, 498, 512, 545, 578, 602, 623, 641 } }
                        },
                        new Dictionary<string, object> {
                            { "product_id", "PROD-10003" },
                            { "name", "Gaming Console" },
                            { "monthly_sales", new[] { 98, 104, 112, 120, 134, 143, 157, 165, 172, 178, 183, 192, 198, 205, 214 } }
                        }
                    }},
                    { "lead_times", new Dictionary<string, object> {
                        { "supplier_1", 45 },
                        { "supplier_2", 30 },
                        { "supplier_3", 60 }
                    }},
                    { "holding_costs", 0.25 }, // 25% of product value per year
                    { "stockout_costs", 1.5 }  // 150% of product value
                };
                
                // Step 1: Generate demand forecast
                var forecastCapability = forecastCapabilities[0].Capability;
                var forecastServer = forecastCapabilities[0].Server;
                
                var forecastRequest = new McpRequest
                {
                    CapabilityId = forecastCapability.Id,
                    Input = new Dictionary<string, object>
                    {
                        { "historical_data", historicalSalesData },
                        { "forecast_periods", 6 }, // 6 months ahead
                        { "seasonality", true },
                        { "external_factors", new Dictionary<string, object> {
                            { "holiday_season", new[] { "November", "December" } },
                            { "product_promotions", new[] {
                                new Dictionary<string, object> {
                                    { "product_id", "PROD-10001" },
                                    { "start_date", "2025-05-15" },
                                    { "end_date", "2025-05-30" },
                                    { "discount_percentage", 15 }
                                }
                            }}
                        }}
                    }
                };
                
                var forecastResponse = await mcpClient.InvokeCapabilityAsync(forecastServer.Url, forecastRequest);
                
                if (forecastResponse.Success)
                {
                    Console.WriteLine("Demand forecasting successful");
                    var forecasts = forecastResponse.Output["forecasts"] as Dictionary<string, object>;
                    
                    Console.WriteLine("\nSales forecasts for next 6 months:");
                    var productForecasts = forecasts["product_forecasts"] as IEnumerable<Dictionary<string, object>>;
                    foreach (var product in productForecasts)
                    {
                        Console.WriteLine($"Product: {product["name"]}");
                        Console.WriteLine($"Forecasted sales: {string.Join(", ", product["forecasted_sales"] as IEnumerable<int>)}");
                        Console.WriteLine($"Confidence interval: {product["confidence_lower"]} - {product["confidence_upper"]}");
                        Console.WriteLine();
                    }
                    
                    // Step 2: Optimize inventory based on forecasts
                    if (inventoryCapabilities.Count > 0)
                    {
                        var inventoryCapability = inventoryCapabilities[0].Capability;
                        var inventoryServer = inventoryCapabilities[0].Server;
                        
                        var inventoryRequest = new McpRequest
                        {
                            CapabilityId = inventoryCapability.Id,
                            Input = new Dictionary<string, object>
                            {
                                { "forecasts", forecasts },
                                { "current_inventory", new[] {
                                    new Dictionary<string, object> {
                                        { "product_id", "PROD-10001" },
                                        { "quantity", 85 },
                                        { "reorder_point", 50 },
                                        { "economic_order_quantity", 100 }
                                    },
                                    new Dictionary<string, object> {
                                        { "product_id", "PROD-10002" },
                                        { "quantity", 210 },
                                        { "reorder_point", 150 },
                                        { "economic_order_quantity", 300 }
                                    },
                                    new Dictionary<string, object> {
                                        { "product_id", "PROD-10003" },
                                        { "quantity", 65 },
                                        { "reorder_point", 40 },
                                        { "economic_order_quantity", 80 }
                                    }
                                }},
                                { "optimization_goals", new[] { "minimize_stockouts", "minimize_holding_cost" } },
                                { "service_level", 0.95 } // 95% service level
                            }
                        };
                        
                        var inventoryResponse = await mcpClient.InvokeCapabilityAsync(inventoryServer.Url, inventoryRequest);
                        
                        if (inventoryResponse.Success)
                        {
                            Console.WriteLine("Inventory optimization successful");
                            var optimization = inventoryResponse.Output["optimization_results"] as Dictionary<string, object>;
                            
                            Console.WriteLine("\nOptimized inventory parameters:");
                            var productParameters = optimization["product_parameters"] as IEnumerable<Dictionary<string, object>>;
                            foreach (var product in productParameters)
                            {
                                Console.WriteLine($"Product: {product["product_id"]}");
                                Console.WriteLine($"Optimized reorder point: {product["reorder_point"]}");
                                Console.WriteLine($"Optimized order quantity: {product["order_quantity"]}");
                                Console.WriteLine($"Safety stock: {product["safety_stock"]}");
                                Console.WriteLine($"Expected service level: {product["expected_service_level"]}");
                                Console.WriteLine();
                            }
                            
                            // Step 3: Integrate with SAP system
                            Console.WriteLine("\nIntegrating with SAP system:");
                            Console.WriteLine("Updating material master records with new reorder points...");
                            Console.WriteLine("Creating purchase requisitions for items below reorder point...");
                            Console.WriteLine("Updating inventory planning parameters...");
                            Console.WriteLine("Sending optimization report to supply chain managers...");
                            
                            // If we have risk analysis capabilities, analyze supply chain risks
                            if (riskCapabilities.Count > 0)
                            {
                                Console.WriteLine("\nAnalyzing supply chain risks...");
                                
                                var riskCapability = riskCapabilities[0].Capability;
                                var riskServer = riskCapabilities[0].Server;
                                
                                var riskRequest = new McpRequest
                                {
                                    CapabilityId = riskCapability.Id,
                                    Input = new Dictionary<string, object>
                                    {
                                        { "supply_chain_data", new Dictionary<string, object> {
                                            { "suppliers", new[] {
                                                new Dictionary<string, object> {
                                                    { "id", "supplier_1" },
                                                    { "name", "Electronics Manufacturing Ltd." },
                                                    { "location", "Shenzhen, China" },
                                                    { "products", new[] { "PROD-10001", "PROD-10003" } }
                                                },
                                                new Dictionary<string, object> {
                                                    { "id", "supplier_2" },
                                                    { "name", "Audio Devices Inc." },
                                                    { "location", "Taipei, Taiwan" },
                                                    { "products", new[] { "PROD-10002" } }
                                                },
                                                new Dictionary<string, object> {
                                                    { "id", "supplier_3" },
                                                    { "name", "Global Tech Components" },
                                                    { "location", "Kuala Lumpur, Malaysia" },
                                                    { "products", new[] { "PROD-10001", "PROD-10002" } }
                                                }
                                            }},
                                            { "logistics_routes", new[] {
                                                new Dictionary<string, object> {
                                                    { "origin", "Shenzhen, China" },
                                                    { "destination", "Los Angeles, USA" },
                                                    { "mode", "sea" },
                                                    { "transit_time_days", 28 }
                                                },
                                                new Dictionary<string, object> {
                                                    { "origin", "Taipei, Taiwan" },
                                                    { "destination", "Los Angeles, USA" },
                                                    { "mode", "air" },
                                                    { "transit_time_days", 5 }
                                                }
                                            }}
                                        }},
                                        { "external_risk_factors", new Dictionary<string, object> {
                                            { "geopolitical", 0.6 }, // Medium-high
                                            { "natural_disasters", 0.4 }, // Medium
                                            { "labor_disruptions", 0.3 } // Medium-low
                                        }}
                                    }
                                };
                                
                                var riskResponse = await mcpClient.InvokeCapabilityAsync(riskServer.Url, riskRequest);
                                
                                if (riskResponse.Success)
                                {
                                    Console.WriteLine("Risk analysis successful");
                                    var riskAnalysis = riskResponse.Output["risk_analysis"] as Dictionary<string, object>;
                                    
                                    Console.WriteLine("\nSupply chain risk assessment:");
                                    Console.WriteLine($"Overall risk score: {riskAnalysis["overall_risk_score"]}");
                                    
                                    var supplierRisks = riskAnalysis["supplier_risks"] as IEnumerable<Dictionary<string, object>>;
                                    Console.WriteLine("\nSupplier risks:");
                                    foreach (var supplierRisk in supplierRisks)
                                    {
                                        Console.WriteLine($"Supplier: {supplierRisk["supplier_name"]}");
                                        Console.WriteLine($"Risk score: {supplierRisk["risk_score"]}");
                                        Console.WriteLine($"Primary risk factors: {string.Join(", ", supplierRisk["risk_factors"] as IEnumerable<string>)}");
                                        Console.WriteLine();
                                    }
                                    
                                    var recommendations = riskAnalysis["recommendations"] as IEnumerable<string>;
                                    Console.WriteLine("\nRisk mitigation recommendations:");
                                    foreach (var recommendation in recommendations)
                                    {
                                        Console.WriteLine($"- {recommendation}");
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
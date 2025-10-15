using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace Api_Orbis_Project.Services
{
    public interface IHuggingFaceService
    {
        Task<string> AskAsync(string prompt, string language = "es");
        Task<string> GetGeoJsonAsync(string countryCode, string category = "general");
    }

    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public HuggingFaceService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["HuggingFace:ApiKey"] 
                ?? throw new Exception("❌ HuggingFace ApiKey not found in configuration.");
            _model = config["HuggingFace:Model"] ?? "Kwaipilot/KAT-Dev";
        }

        public async Task<string> AskAsync(string prompt, string language = "es")
        {
            try
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://router.huggingface.co/v1/chat/completions"
                );

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var payload = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = $"Eres un asistente especializado en análisis de datos geográficos. Responde siempre en {language}."
                        },
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    max_tokens = 4096, 
                    temperature = 0.1  
                };

                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"⚠️ Hugging Face API error: {response.StatusCode}");
                    return null;
                }

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                var text = root.GetProperty("choices")[0]
                               .GetProperty("message")
                               .GetProperty("content")
                               .GetString();

                return text ?? "No response generated.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Exception en AskAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetGeoJsonAsync(string countryCode, string category = "general")
        {
            // Prompts específicos para cada categoría
            string prompt = category.ToLower() switch
            {
                "salud" => GetSaludPrompt(countryCode),
                "seguridad" => GetSeguridadPrompt(countryCode),
                "cultura" => GetCulturaPrompt(countryCode),
                _ => GetGeneralPrompt(countryCode, category)
            };

            var result = await AskAsync(prompt, "es");

            // Si la API falla, usar datos hardcodeados específicos
            if (string.IsNullOrEmpty(result))
            {
                Console.WriteLine($"⚠️ API falló, usando fallback para {countryCode}/{category}");
                return GetFallbackGeoJson(countryCode, category);
            }

            return result;
        }

        private string GetSaludPrompt(string countryCode)
        {
            return $@"
            Genera un GeoJSON FeatureCollection con ZONAS DE RIESGO DE SALUD para {countryCode}.
            
            ENFOQUE: Mostrar áreas con problemas de salud pública como:
            - Zonas con brotes de dengue, malaria, zika
            - Áreas con alta contaminación del aire/agua
            - Sectores con falta de acceso a servicios de salud
            - Lugares con alta incidencia de enfermedades respiratorias
            
            ESTRUCTURA REQUERIDA:
            - Geometry type: Point (para ubicaciones específicas)
            - Propiedades OBLIGATORIAS:
              * name: Nombre descriptivo del riesgo
              * category: 'salud'
              * risk_type: 'dengue' | 'contaminacion' | 'acceso_salud' | 'respiratorias'
              * risk_level: 'low' | 'medium' | 'high' | 'critical'
              * description: Breve descripción del riesgo
              * radius: Radio de afectación en metros (ej: 500, 1000, 2000)
            
            EJEMPLO:
            {{
                ""type"": ""FeatureCollection"",
                ""features"": [
                    {{
                        ""type"": ""Feature"",
                        ""properties"": {{
                            ""name"": ""Brote Dengue - La Carpio"",
                            ""category"": ""salud"",
                            ""risk_type"": ""dengue"",
                            ""risk_level"": ""high"",
                            ""description"": ""Alta incidencia de casos de dengue"",
                            ""radius"": 800
                        }},
                        ""geometry"": {{
                            ""type"": ""Point"",
                            ""coordinates"": [-84.095, 9.945]
                        }}
                    }}
                ]
            }}
            
            Genera 3-4 features con coordenadas realistas para {countryCode}.
            SOLO JSON, sin explicaciones.
            ";
        }

        private string GetSeguridadPrompt(string countryCode)
        {
            return $@"
            Genera un GeoJSON FeatureCollection con ZONAS DE RIESGO DE SEGURIDAD para {countryCode}.
            
            ENFOQUE: Mostrar áreas con problemas de seguridad como:
            - Barrios con alta tasa de robos/asaltos
            - Zonas con presencia de narcotráfico
            - Sectores con alta criminalidad nocturna
            - Áreas con conflictos sociales
            
            ESTRUCTURA REQUERIDA:
            - Geometry type: Point (para ubicaciones específicas)
            - Propiedades OBLIGATORIAS:
              * name: Nombre descriptivo del riesgo
              * category: 'seguridad'
              * risk_type: 'robos' | 'narcotrafico' | 'nocturno' | 'conflicto_social'
              * risk_level: 'low' | 'medium' | 'high' | 'critical'
              * description: Breve descripción del riesgo
              * radius: Radio de peligro en metros (ej: 300, 500, 1000)
              * time_risk: 'day' | 'night' | 'both'
            
            EJEMPLO:
            {{
                ""type"": ""FeatureCollection"",
                ""features"": [
                    {{
                        ""type"": ""Feature"",
                        ""properties"": {{
                            ""name"": ""Zona Roja - La Merced"",
                            ""category"": ""seguridad"",
                            ""risk_type"": ""robos"",
                            ""risk_level"": ""high"",
                            ""description"": ""Alta incidencia de robos a turistas"",
                            ""radius"": 500,
                            ""time_risk"": ""both""
                        }},
                        ""geometry"": {{
                            ""type"": ""Point"",
                            ""coordinates"": [-84.0875, 9.9280]
                        }}
                    }}
                ]
            }}
            
            Genera 3-4 features con coordenadas realistas para {countryCode}.
            SOLO JSON, sin explicaciones.
            ";
        }

        private string GetCulturaPrompt(string countryCode)
        {
            return $@"
            Genera un GeoJSON FeatureCollection con SITIOS CULTURALES para {countryCode}.
            
            ENFOQUE: Mostrar lugares de interés cultural como:
            - Museos y galerías de arte
            - Teatros y centros culturales
            - Sitios históricos y monumentos
            - Centros tradicionales y artesanías
            
            ESTRUCTURA REQUERIDA:
            - Geometry type: Point (para ubicaciones específices)
            - Propiedades OBLIGATORIAS:
              * name: Nombre del sitio cultural
              * category: 'cultura'
              * type: 'museo' | 'teatro' | 'monumento' | 'centro_cultural' | 'artesania'
              * description: Breve descripción del lugar
              * importance: 'low' | 'medium' | 'high' | 'national'
            
            EJEMPLO:
            {{
                ""type"": ""FeatureCollection"",
                ""features"": [
                    {{
                        ""type"": ""Feature"",
                        ""properties"": {{
                            ""name"": ""Teatro Nacional"",
                            ""category"": ""cultura"",
                            ""type"": ""teatro"",
                            ""description"": ""Principal teatro del país"",
                            ""importance"": ""national""
                        }},
                        ""geometry"": {{
                            ""type"": ""Point"",
                            ""coordinates"": [-84.0846, 9.9334]
                        }}
                    }}
                ]
            }}
            
            Genera 3-4 features con coordenadas realistas para {countryCode}.
            SOLO JSON, sin explicaciones.
            ";
        }

        private string GetGeneralPrompt(string countryCode, string category)
        {
            return $@"
            Genera un objeto GeoJSON FeatureCollection válido para el país '{countryCode}' en la categoría '{category}'.
            
            REQUISITOS:
            - Solo 3-4 features máximo
            - Coordenadas realistas para {countryCode}
            - Propiedades relevantes para la categoría '{category}'
            - Geometry type: Point
            - JSON válido SIN texto adicional
            
            SOLO JSON, sin ```json, sin ```, sin explicaciones.
            ";
        }

        private string GetFallbackGeoJson(string countryCode, string category)
        {
            if (countryCode.ToUpper() == "CRI")
            {
                switch (category.ToLower())
                {
                    case "salud":
                        return @"{
                            ""type"": ""FeatureCollection"",
                            ""features"": [
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Brote Dengue - La Carpio"",
                                        ""category"": ""salud"",
                                        ""risk_type"": ""dengue"",
                                        ""risk_level"": ""high"",
                                        ""description"": ""Zona con alta incidencia de casos de dengue"",
                                        ""radius"": 800
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.095, 9.945]
                                    }
                                },
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Contaminación Aire - Pavas"",
                                        ""category"": ""salud"",
                                        ""risk_type"": ""contaminacion"",
                                        ""risk_level"": ""medium"",
                                        ""description"": ""Alta contaminación por tráfico e industria"",
                                        ""radius"": 1200
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.135, 9.955]
                                    }
                                },
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Falta Acceso Salud - Los Guido"",
                                        ""category"": ""salud"",
                                        ""risk_type"": ""acceso_salud"",
                                        ""risk_level"": ""critical"",
                                        ""description"": ""Zona marginada con poco acceso a servicios de salud"",
                                        ""radius"": 1500
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.055, 9.900]
                                    }
                                }
                            ]
                        }";

                    case "seguridad":
                        return @"{
                            ""type"": ""FeatureCollection"",
                            ""features"": [
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Zona Roja - La Merced"",
                                        ""category"": ""seguridad"",
                                        ""risk_type"": ""robos"",
                                        ""risk_level"": ""high"",
                                        ""description"": ""Alta incidencia de robos y asaltos"",
                                        ""radius"": 500,
                                        ""time_risk"": ""both""
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.0875, 9.9280]
                                    }
                                },
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Narcotráfico - El Infiernillo"",
                                        ""category"": ""seguridad"",
                                        ""risk_type"": ""narcotrafico"",
                                        ""risk_level"": ""critical"",
                                        ""description"": ""Zona con presencia de narcotráfico"",
                                        ""radius"": 300,
                                        ""time_risk"": ""night""
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.0650, 9.9200]
                                    }
                                },
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Robos Nocturnos - Barrio México"",
                                        ""category"": ""seguridad"",
                                        ""risk_type"": ""robos"",
                                        ""risk_level"": ""medium"",
                                        ""description"": ""Robos frecuentes durante la noche"",
                                        ""radius"": 400,
                                        ""time_risk"": ""night""
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.0950, 9.9400]
                                    }
                                }
                            ]
                        }";

                    case "cultura":
                        return @"{
                            ""type"": ""FeatureCollection"",
                            ""features"": [
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Teatro Nacional de Costa Rica"",
                                        ""category"": ""cultura"",
                                        ""type"": ""teatro"",
                                        ""description"": ""Principal teatro y monumento histórico del país"",
                                        ""importance"": ""national""
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.0846, 9.9334]
                                    }
                                },
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Museo Nacional de Costa Rica"",
                                        ""category"": ""cultura"",
                                        ""type"": ""museo"",
                                        ""description"": ""Museo de historia y cultura costarricense"",
                                        ""importance"": ""national""
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.0803, 9.9341]
                                    }
                                },
                                {
                                    ""type"": ""Feature"",
                                    ""properties"": {
                                        ""name"": ""Museo de Arte Costarricense"",
                                        ""category"": ""cultura"",
                                        ""type"": ""museo"",
                                        ""description"": ""Museo dedicado al arte costarricense"",
                                        ""importance"": ""high""
                                    },
                                    ""geometry"": {
                                        ""type"": ""Point"",
                                        ""coordinates"": [-84.1125, 9.9978]
                                    }
                                }
                            ]
                        }";
                }
            }

            // Fallback genérico
            return $@"{{
                ""type"": ""FeatureCollection"",
                ""features"": [
                    {{
                        ""type"": ""Feature"",
                        ""properties"": {{
                            ""name"": ""Punto de ejemplo"",
                            ""category"": ""{category}"",
                            ""country"": ""{countryCode}""
                        }},
                        ""geometry"": {{
                            ""type"": ""Point"",
                            ""coordinates"": [0, 0]
                        }}
                    }}
                ]
            }}";
        }
    }
}
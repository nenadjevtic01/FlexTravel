using FlexTravel.Application.Constants;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.UseCases.Commands.SampleData;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class FillDatabaseCommandEf : IFillDatabaseCommand
    {
        private readonly FlexTravelContext _context;

        public FillDatabaseCommandEf(FlexTravelContext context)
        {
            _context = context;
        }
        public int UseCaseId => UseCaseIds.FillDatabase;

        public string UseCaseName => "Fill database";

        public string UseCaseDescription => "Initial fill database";

        public void Execute(int request)
        {
            // Checked on data, not on AuditLog: UseCaseHandler writes the audit entry before Execute runs.
            if (_context.Continent.Any())
            {
                throw new ConflictException("Database already filled.");
            }

            var continents = new List<Continent>
            {
                //Europe
                new Continent
                {
                    ContinentName="Europe",
                },
                //Asia
                new Continent
                {
                    ContinentName="Asia",
                },
                //Africa
                new Continent
                {
                    ContinentName="Africa",
                },
                //South America
                new Continent
                {
                    ContinentName="South America",
                },
                //North America
                new Continent
                {
                    ContinentName="North America",
                },
                //Australia
                new Continent
                {
                    ContinentName="Oceania",
                },
                //Antarctica
                new Continent
                {
                    ContinentName="Antarctica"
                }
            };

            var country = new List<Country>
            {
                //Cuba
                new Country
                {
                    CountryName="Cuba",
                    CapitalCity="Havana",
                    Population=11186467,
                    TotalArea=109884,
                    Currency="CUP - Cuban Peso",
                    Language="Spanish",
                    Continent=continents[4],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Card",
                        Duration="Up to 30 days.",
                        Requirements="Completed application form and supporting documents, including proof of accommodation reservation, flight ticket, and health insurance.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=5,
                        Advices="The situation in Cuba is safe and the crime rate is very low. In any case, caution is advised against petty theft and pickpocketing in large cities and tourist areas. Walking on the street is free and without problems even in the evening hours. Drugs are an illegal phenomenon, and as such punishable by law."
                    }
                },
                //Dominican Republic
                new Country
                {
                    CountryName="Dominican Republic",
                    CapitalCity="Santo Domingo",
                    Population=11332972,
                    TotalArea=48671,
                    Currency="DOP - Dominican peso",
                    Language="Spanish",
                    Continent=continents[4],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist visa",
                        Duration="Varies, typically up to 30 days",
                        Requirements="Completed application form, copy of accommodation reservation, and proof of financial ability for the duration of stay."
                    },
                    Safety=new Safety
                    {
                        SafetyLevel = 3,
                        Advices="Leaving tourist zones is not advised, because of possible theft or kidnapping. Safety in tourist zones and hotel area is good."
                    }
                }, 
                //Egypt
                new Country
                {
                    CountryName="Egypt",
                    CapitalCity="Cairo",
                    Population=112716598,
                    TotalArea=1002450,
                    Currency="EGP - Egyptian pound",
                    Language="Arabic",
                    Continent=continents[2],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist visa",
                        Duration="Usually up to 30 days.",
                        Requirements="Completed application form, original passport valid for at least 6 months, passport photo, proof of financial ability, and proof of accommodation reservation or invitation letter."
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=4,
                        Advices="The security situation is relatively stable. Special attention is given to tourist destinations, considering that the Arab Republic of Egypt is one of the most visited countries in the world. Over 30 million Egyptians are directly or indirectly involved in the tourism industry."
                    }
                },
                //France
                new Country
                {
                    CountryName="France",
                    CapitalCity="Paris",
                    Population=67000000,
                    TotalArea=551695,
                    Currency="EUR - Euro",
                    Language= "French",
                    Continent=continents[0],
                    VisaInfo=new Visa
                    {
                        VisaType="Schengen Visa (Short Stay)",
                        Duration="90 days",
                        Requirements="Valid passport, travel itinerary, proof of accommodation, travel insurance, proof of financial means."
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="France is generally considered safe for tourists, but vigilance is advised, particularly in crowded areas. There have been occasional security concerns, including terrorism incidents. Travelers should stay informed, take standard safety precautions, and heed local authorities' advice."
                    }
                },
                //Turkey
                new Country
                {
                    CountryName="Turkey",
                    CapitalCity="Ankara",
                    Population=83000000,
                    TotalArea=783356,
                    Currency="TRY - Turkish Lira",
                    Language="Turkish",
                    Continent=continents[1],
                    VisaInfo=new Visa
                    {
                        VisaType="E-Visa(Touristic)",
                        Duration="30 days",
                        Requirements="Valid passport, completed online application, travel itinerary, hotel reservation, return ticket, e-Visa fee.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Turkey is a culturally rich destination generally safe for tourists. However, awareness of surroundings, especially in crowded places, is recommended. Some regions, particularly near the Syrian border, may have security concerns. Staying updated on current events, respecting local customs, and following local advice contribute to a safer experience. Travelers should check for any travel advisories before planning their visit."
                    }
                },
                //Brazil
                new Country
                {
                    CountryName="Brazil",
                    CapitalCity="Brasília",
                    Population=213000000,
                    TotalArea=8515767,
                    Currency="BRL - Brazilian Real",
                    Language="Portuguese",
                    Continent=continents[3],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="90 days (extendable)",
                        Requirements="Valid passport, visa application, passport photos, travel itinerary, proof of financial means."
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Exercise caution in urban areas, be aware of your surroundings, and avoid displaying valuable items in public."
                    }
                },
                //Greece
                new Country
                {
                    CountryName="Greece",
                    CapitalCity="Athens",
                    Population=10400000,
                    TotalArea=131957,
                    Currency="EUR - Euro",
                    Language="Greek",
                    Continent=continents[0],
                    VisaInfo=new Visa
                    {
                        VisaType="Schengen Visa (Short Stay)",
                        Duration="90 days",
                        Requirements="Valid passport, travel itinerary, proof of accommodation, travel insurance, proof of financial means.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=4,
                        Advices="Watch out for pickpockets in tourist areas, be cautious on public transportation, and stay informed about local safety conditions."
                    }
                },
                //Mexico
                new Country
                {
                    CountryName="Mexico",
                    CapitalCity="Mexico City",
                    Population=126000000,
                    TotalArea=1964375,
                    Currency="MXN - Mexican Peso",
                    Language="Spanish",
                    Continent=continents[4],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="180 days",
                        Requirements="Valid passport, visa application, passport photos, travel itinerary, proof of financial means."
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Exercise caution in certain regions, use reputable transportation services, and avoid traveling at night in unfamiliar areas."
                    }
                },
                //Russia
                new Country
                {
                    CountryName="Russia",
                    CapitalCity="Moscow",
                    Population=145000000,
                    TotalArea=17098242,
                    Currency="RUB - Russian Ruble",
                    Language="Russian",
                    Continent=continents[0],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="30 days",
                        Requirements="Valid passport, visa application, invitation letter from a Russian host, travel insurance.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Be cautious in crowded places, follow local laws, and stay informed about potential risks in different regions."
                    }
                },
                //India
                new Country
                {
                    CountryName="India",
                    CapitalCity="New Delhi",
                    Population=1300000000,
                    TotalArea=3287263,
                    Currency="INR - Indian Rupee",
                    Language="Hindi, English",
                    Continent=continents[1],
                    VisaInfo=new Visa
                    {
                        VisaType="e-Tourist Visa (for short stays)",
                        Duration="60 days",
                        Requirements="Valid passport, return ticket, proof of accommodation, travel itinerary.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Be vigilant in crowded areas, use reputable transportation, and respect local customs and traditions."
                    }
                },
                //Peru
                new Country
                {
                    CountryName="Peru",
                    CapitalCity="Lima",
                    Population=33000000,
                    TotalArea=1285216,
                    Currency="PEN - Peruvian Sol",
                    Language="Spanish",
                    Continent=continents[3],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="183 days (extendable)",
                        Requirements="Valid passport, visa application, passport photos, travel itinerary, proof of financial means.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=4,
                        Advices="Be cautious in crowded places, secure your belongings, and stay informed about safety conditions in specific regions."
                    }
                },
                //South Korea
                new Country
                {
                    CountryName="South Korea",
                    CapitalCity="Seoul",
                    Population=51000000,
                    TotalArea=505990,
                    Currency="KRW - Korean Won",
                    Language="Korean",
                    Continent=continents[1],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="90 days",
                        Requirements="Valid passport, visa application, passport photos, travel itinerary, proof of financial means.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=5,
                        Advices=": South Korea is generally safe. Follow standard safety practices, and enjoy the modern and traditional aspects of the country."
                    }
                },
                //Morocco
                new Country
                {
                    CountryName="Morocco",
                    CapitalCity="Rabat",
                    Population=37000000,
                    TotalArea=446550,
                    Currency="MAD - Moroccan Dirham",
                    Language="Arabic",
                    Continent=continents[2],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="90 days",
                        Requirements="Valid passport, visa application, passport photos, travel itinerary, proof of accommodation.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Exercise caution in crowded places, be aware of pickpockets, and respect local customs."
                    }
                },
                //Argentina
                new Country
                {
                    CountryName="Argentina",
                    CapitalCity="Buenos Aires",
                    Population=45000000,
                    TotalArea=2780000,
                    Currency="ARS - Argentine Peso",
                    Language="Spanish",
                    Continent=continents[3],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="Up to 90 days",
                        Requirements="Valid passport, proof of accommodation, return ticket, proof of financial means.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Traveling in most parts of Argentina is generally safe, especially in tourist areas like Buenos Aires. However, be cautious of petty theft, especially in crowded places. Exercise vigilance in nighttime city areas and be mindful of carrying valuable items."
                    }
                },
                //Tanzania
                new Country
                {
                    CountryName="Tanzania",
                    CapitalCity="Dodoma",
                    Population=61000000,
                    TotalArea=947303,
                    Currency="TZS - Tanzanian Shilling",
                    Language="Swahili",
                    Continent=continents[2],
                    VisaInfo=new Visa
                    {
                        VisaType="Tourist Visa",
                        Duration="90 days",
                        Requirements="Common requirements include a valid passport, completed visa application form, passport-sized photos, proof of accommodation, return ticket, and proof of funds."
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Tanzania is relatively safe, but tourists should be aware of the possibility of petty theft and scams. Take care of valuable belongings, especially in crowded places. Adhere to local customs, dress modestly when visiting religious sites, and follow health safety advice in specific regions."
                    }
                },
                //Australia
                new Country
                {
                    CountryName="Australia",
                    CapitalCity="Canberra",
                    Population=25000000,
                    TotalArea=7690000,
                    Currency="AUD - Australian Dollar",
                    Language="English",
                    Continent=continents[5],
                    VisaInfo=new Visa
                    {
                        VisaType="Electronic Travel Authority (ETA) or Visitor Visa (Subclass 600) for tourism purposes.",
                        Duration="ETA allows stays of up to 3 months, and the Visitor Visa can vary in duration.",
                        Requirements="Valid passport, health and character requirements, proof of funds, and a return ticket. ETA is available online, while the Visitor Visa application may require additional documentation."
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=5,
                        Advices="Australia is generally considered a safe destination with low crime rates. Exercise normal precautions, be aware of your surroundings, and take common-sense safety measures. Respect local laws and customs."
                    }
                },
                //Antarctica
                new Country
                {
                    CountryName="Antarctica",
                    CapitalCity=null,
                    Population=null,
                    TotalArea=14000000,
                    Currency=null,
                    Language=null,
                    Continent=continents[6],
                    VisaInfo=new Visa
                    {
                        VisaType="Not needed",
                        Duration="/",
                        Requirements="/",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=2,
                        Advices="Antarctica poses unique challenges due to its harsh climate and isolation. Safety is a priority, and visitors should adhere to guidelines provided by experienced guides and tour operators. Be prepared for extreme weather conditions, follow safety protocols, and ensure you have the appropriate gear. Emergency services are limited, emphasizing the importance of caution and preparedness. Respect environmental regulations to preserve the delicate ecosystem. Always stay informed about local conditions and emergency procedures."
                    }
                },
                //Spain
                new Country
                {
                    CountryName="Spain",
                    CapitalCity="Madrid",
                    Population=47000000,
                    TotalArea=505990,
                    Currency="EUR - Euro",
                    Language="Spanish",
                    Continent=continents[0],
                    VisaInfo=new Visa
                    {
                        VisaType="Schengen Visa",
                        Duration="Up to 90 days",
                        Requirements="Passport, accommodation confirmation, return ticket, insurance.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=3,
                        Advices="Keep personal belongings secure in public places, exercise caution in tourist areas, stay informed about local safety news."
                    }
                },
                //Italy
                new Country
                {
                    CountryName="Italy",
                    CapitalCity="Roe",
                    Population=60000000,
                    TotalArea=301340,
                    Currency="EUR - Euro",
                    Language="Italian",
                    Continent=continents[0],
                    VisaInfo=new Visa
                    {
                        VisaType="Schengen Visa",
                        Duration="Up to 90 days",
                        Requirements="Passport, accommodation confirmation, return ticket, insurance.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=4,
                        Advices="Be cautious in crowded places, especially in tourist areas. Stay updated on tourist scams and keep personal belongings secure."
                    }
                },
                //Switzerland
                new Country
                {
                    CountryName="Switzerland",
                    CapitalCity="Bern",
                    Population=8000000,
                    TotalArea=41290,
                    Currency="CHF - Swiss Franc",
                    Language="German, French, Italian",
                    Continent=continents[0],
                    VisaInfo=new Visa
                    {
                        VisaType="Schengen Visa",
                        Duration="Up to 90 days",
                        Requirements="Passport, accommodation confirmation, return ticket, insurance.",
                    },
                    Safety=new Safety
                    {
                        SafetyLevel=5,
                        Advices="Switzerland is known for its high safety levels, but it's still advisable to follow local guidelines and keep personal belongings secure."
                    }
                },
            };

            var destinations = new List<Destination>
            {
                //Cuba -> Havana
                new Destination
                {
                    DestinationName="Havana",
                    DestinationDescription="The capital city known for its vibrant culture, historic architecture, and lively atmosphere.",
                    Country=country[0],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Cuba/Havana/Havana01.jpg",
                            Alt="Havana image 1"
                        },
                        new DestinationImage
                        {
                            Src="Cuba/Havana/Havana02.jpg",
                            Alt="Havana image 2"
                        },
                        new DestinationImage
                        {
                            Src="Cuba/Havana/Havana03.jpg",
                            Alt="Havana image 3"
                        },
                    }
                },
                //Cuba -> Varadero
                new Destination
                {
                    DestinationName="Varadero",
                    DestinationDescription="A popular beach resort town with stunning white sandy beaches and turquoise waters.",
                    Country=country[0],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Cuba/Varadero/Varadero01.jpg",
                            Alt="Varadero image 1"
                        },
                        new DestinationImage
                        {
                            Src="Cuba/Varadero/Varadero02.jpg",
                            Alt="Varadero image 2"
                        },
                        new DestinationImage
                        {
                            Src="Cuba/Varadero/Varadero03.jpg",
                            Alt="Varadero image 3"
                        },
                    }
                },
                //Cuba -> Trinidad
                new Destination
                {
                    DestinationName="Trinidad",
                    DestinationDescription="A UNESCO World Heritage Site, Trinidad is famous for its well-preserved Spanish colonial architecture and cobblestone streets.",
                    Country=country[0],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Cuba/Trinidad/Trinidad01.jpg",
                            Alt="Trinidad image 1"
                        },
                        new DestinationImage
                        {
                            Src="Cuba/Trinidad/Trinidad02.jpg",
                            Alt="Trinidad image 2"
                        },
                        new DestinationImage
                        {
                            Src="Cuba/Trinidad/Trinidad03.jpg",
                            Alt="Trinidad image 3"
                        },
                    }
                },
                //Dominican Republic -> Punta Cana
                new Destination
                {
                    DestinationName="Punta Cana",
                    DestinationDescription="Punta Cana is the most beautiful and popular destination in the Dominican Republic with almost 100 miles of the best beaches in the world. Surrounded by magnificent world-class resorts, spectacular turquoise waters, white sand and beautiful palm trees, the coast attracts guests from all over the world.",
                    Country=country[1],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="DominicanRepublic/PuntaCana/PuntaCana01.jpg",
                            Alt="Punta Cana image 1"
                        },
                        new DestinationImage
                        {
                            Src="DominicanRepublic/PuntaCana/PuntaCana02.jpg",
                            Alt="Punta Cana image 2"
                        },
                        new DestinationImage
                        {
                            Src="DominicanRepublic/PuntaCana/PuntaCana03.jpg",
                            Alt="Punta Cana image 3"
                        },
                    }
                },
                //Egypt -> Cairo
                new Destination
                {
                    DestinationName="Cairo",
                    DestinationDescription="The capital and largest city, home to iconic landmarks such as the Pyramids of Giza and the Sphinx. Cairo offers a vibrant blend of history, culture, and modern urban life.",
                    Country=country[2],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Egypt/Cairo/Cairo01.jpg",
                            Alt="Cairo image 1"
                        },
                        new DestinationImage
                        {
                            Src="Egypt/Cairo/Cairo02.jpg",
                            Alt="Cairo image 2"
                        },
                        new DestinationImage
                        {
                            Src="Egypt/Cairo/Cairo03.jpg",
                            Alt="Cairo image 3"
                        },
                    }
                },
                //Dominican Republic -> Santo Domingo
                new Destination
                {
                    DestinationName="Santo Domingo",
                    DestinationDescription="Santo Domingo, the capital city, is a UNESCO World Heritage Site with a rich history. Explore the Colonial Zone, filled with cobblestone streets and historic landmarks, including the oldest cathedral in the Americas and the Alcazar de Colon.",
                    Country=country[1],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="DominicanRepublic/SantoDomingo/SantoDomingo01.jpg",
                            Alt="Santo Domingo image 1"
                        },
                        new DestinationImage
                        {
                            Src="DominicanRepublic/SantoDomingo/SantoDomingo02.jpg",
                            Alt="Santo Domingo image 2"
                        },
                        new DestinationImage
                        {
                            Src="DominicanRepublic/SantoDomingo/SantoDomingo03.jpg",
                            Alt="Santo Domingo image 3"
                        },
                    }
                },
                //Dominican Republic -> Saona Island
                new Destination
                {
                    DestinationName="Saona Island",
                    DestinationDescription="Saona Island, located off the southeast coast, is a tropical paradise with white sandy beaches and crystal-clear waters. Visitors can take boat excursions to enjoy the natural beauty, go snorkeling, and experience the laid-back Caribbean atmosphere.",
                    Country=country[1],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="DominicanRepublic/SaonaIsland/SaonaIsland01.jpg",
                            Alt="Saona Island image 1"
                        },
                        new DestinationImage
                        {
                            Src="DominicanRepublic/SaonaIsland/SaonaIsland02.jpg",
                            Alt="Saona Island image 2"
                        },
                        new DestinationImage
                        {
                            Src="DominicanRepublic/SaonaIsland/SaonaIsland03.jpg",
                            Alt="Saona Island image 3"
                        },
                    }
                },
                //Egypt -> Luxor
                new Destination
                {
                    DestinationName="Luxor",
                    DestinationDescription="Luxor, often referred to as the \"world's greatest open-air museum,\" boasts ancient temples and monuments, including the Valley of the Kings and Karnak Temple. It offers a glimpse into Egypt's pharaonic past and is a must-visit for history enthusiasts.",
                    Country=country[2],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Egypt/Luxor/Luxor01.jpg",
                            Alt="Luxor image 1"
                        },
                        new DestinationImage
                        {
                            Src="Egypt/Luxor/Luxor02.jpg",
                            Alt="Luxor image 2"
                        },
                        new DestinationImage
                        {
                            Src="Egypt/Luxor/Luxor03.jpg",
                            Alt="Luxor image 3"
                        },
                    }
                },
                //Egypt -> Sharm El Sheikh
                new Destination
                {
                    DestinationName="Sharm El Sheikh",
                    DestinationDescription="Sharm El Sheikh, situated on the Red Sea coast, is a popular resort destination known for its world-class diving and snorkeling sites. The vibrant coral reefs, clear waters, and diverse marine life make it a paradise for water sports enthusiasts and beach lovers.",
                    Country=country[2],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Egypt/SharmElSheikh/SharmElSheikh01.jpg",
                            Alt="Sharm El Sheikh image 1"
                        },
                        new DestinationImage
                        {
                            Src="Egypt/SharmElSheikh/SharmElSheikh02.jpg",
                            Alt="Sharm El Sheikh image 2"
                        },
                        new DestinationImage
                        {
                            Src="Egypt/SharmElSheikh/SharmElSheikh03.jpg",
                            Alt="Sharm El Sheikh image 3"
                        },
                    }
                },
                //France -> Paris
                new Destination
                {
                    DestinationName="Paris",
                    DestinationDescription="The City of Lights, renowned for its romantic ambiance, iconic landmarks like the Eiffel Tower, Louvre Museum, and charming neighborhoods such as Montmartre.",
                    Country=country[3],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="France/Paris/Paris01.jpg",
                            Alt="Paris image 1"
                        },
                        new DestinationImage
                        {
                            Src="France/Paris/Paris02.jpg",
                            Alt="Paris image 2"
                        },
                        new DestinationImage
                        {
                            Src="France/Paris/Paris03.jpg",
                            Alt="Paris image 3"
                        },
                    }
                },
                //France -> Nice
                new Destination
                {
                    DestinationName="Nice",
                    DestinationDescription="Nestled on the French Riviera, Nice boasts the Promenade des Anglais, beautiful beaches, and a vibrant blend of Mediterranean culture.",
                    Country=country[3],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="France/Nice/Nice01.jpg",
                            Alt="Nice image 1"
                        },
                        new DestinationImage
                        {
                            Src="France/Nice/Nice02.jpg",
                            Alt="Nice image 2"
                        },
                        new DestinationImage
                        {
                            Src="France/Nice/Nice03.jpg",
                            Alt="Nice image 3"
                        },
                    }
                },
                //France -> Avignon
                new Destination
                {
                    DestinationName="Avignon",
                    DestinationDescription="A city steeped in history, famous for the Palais des Papes and the Pont Saint-Bénézet, offering a glimpse into medieval France.",
                    Country=country[3],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="France/Avignon/Avignon01.jpg",
                            Alt="Avignon image 1"
                        },
                        new DestinationImage
                        {
                            Src="France/Avignon/Avignon02.jpg",
                            Alt="Avignon image 2"
                        },
                        new DestinationImage
                        {
                            Src="France/Avignon/Avignon03.jpg",
                            Alt="Avignon image 3"
                        },
                    }
                },
                //Turkey -> Istanbul
                new Destination
                {
                    DestinationName="Istanbul",
                    DestinationDescription="A transcontinental city straddling Europe and Asia, Istanbul is rich in history with landmarks like the Hagia Sophia, Blue Mosque, and the Grand Bazaar.",
                    Country=country[4],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Turkey/Istanbul/Istanbul01.jpg",
                            Alt="Istanbul image 1"
                        },
                        new DestinationImage
                        {
                            Src="Turkey/Istanbul/Istanbul02.jpg",
                            Alt="Istanbul image 2"
                        },
                        new DestinationImage
                        {
                            Src="Turkey/Istanbul/Istanbul03.jpg",
                            Alt="Istanbul image 3"
                        },
                    }
                },
                //Turkey -> Cappadocia
                new Destination
                {
                    DestinationName="Cappadocia",
                    DestinationDescription="Known for its surreal landscapes, fairy-tale-like chimneys, and cave dwellings, offering unique hot air balloon experiences.",
                    Country=country[4],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Turkey/Cappadocia/Cappadocia01.jpg",
                            Alt="Cappadocia image 1"
                        },
                        new DestinationImage
                        {
                            Src="Turkey/Cappadocia/Cappadocia02.jpg",
                            Alt="Cappadocia image 2"
                        },
                        new DestinationImage
                        {
                            Src="Turkey/Cappadocia/Cappadocia03.jpg",
                            Alt="Cappadocia image 3"
                        },
                    }
                },
                //Turkey -> Antalya
                new Destination
                {
                    DestinationName="Antalya",
                    DestinationDescription="A coastal gem with pristine beaches, ancient ruins like Perge, and the stunning Düden Waterfalls.",
                    Country=country[4],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Turkey/Antalya/Antalya01.jpg",
                            Alt="Antalya image 1"
                        },
                        new DestinationImage
                        {
                            Src="Turkey/Antalya/Antalya02.jpg",
                            Alt="Antalya image 2"
                        },
                        new DestinationImage
                        {
                            Src="Turkey/Antalya/Antalya03.jpg",
                            Alt="Antalya image 3"
                        },
                    }
                },
                //Brazil -> Rio De Janeiro
                new Destination
                {
                    DestinationName="Rio de Janeiro",
                    DestinationDescription="Known for its vibrant Carnival, Copacabana and Ipanema beaches, and the iconic Christ the Redeemer statue atop Corcovado Mountain.",
                    Country=country[5],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Brazil/RioDeJaneiro/RioDeJaneiro01.jpg",
                            Alt="Rio De Janeiro image 1"
                        },
                        new DestinationImage
                        {
                            Src="Brazil/RioDeJaneiro/RioDeJaneiro02.jpg",
                            Alt="Rio De Janeiro image 2"
                        },
                        new DestinationImage
                        {
                            Src="Brazil/RioDeJaneiro/RioDeJaneiro03.jpg",
                            Alt="Rio De Janeiro image 3"
                        },
                    }
                },
                //Brazil -> Florianopolis
                new Destination
                {
                    DestinationName="Florianópolis",
                    DestinationDescription="Florianópolis, known as the \"Magic Island,\" beckons with its beautiful beaches, cultural richness, and vibrant atmosphere. This Brazilian gem seamlessly blends modernity with tradition, offering visitors a delightful mix of sun-soaked relaxation, water sports, local flavors, and dynamic cultural experiences.",
                    Country= country[5],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Brazil/Florianopolis/Florianopolis01.jpg",
                            Alt="Florianopolis image 1"
                        },
                        new DestinationImage
                        {
                            Src="Brazil/Florianopolis/Florianopolis02.jpg",
                            Alt="Florianopolis image 2"
                        },
                        new DestinationImage
                        {
                            Src="Brazil/Florianopolis/Florianopolis03.jpg",
                            Alt="Florianopolis image 3"
                        },
                    }
                },
                //Brazil -> Salvador
                new Destination
                {
                    DestinationName="Salvador",
                    DestinationDescription="Vibrant Afro-Brazilian culture, historic Pelourinho, diverse cuisine, lively Carnival, beautiful beaches like Porto da Barra, and stunning sunsets at Farol da Barra lighthouse. A unique mix of history, culture, and natural beauty.",
                    Country=country[5],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Brazil/Salvador/Salvador01.jpg",
                            Alt="Salvador image 1"
                        },
                        new DestinationImage
                        {
                            Src="Brazil/Salvador/Salvador02.jpg",
                            Alt="Salvador image 2"
                        },
                        new DestinationImage
                        {
                            Src="Brazil/Salvador/Salvador03.jpg",
                            Alt="Salvador image 3"
                        },
                    }
                },
                //Greece -> Athens
                new Destination
                {
                    DestinationName="Athens",
                    DestinationDescription="The cradle of Western civilization, featuring the Acropolis, Parthenon, and the Ancient Agora.",
                    Country=country[6],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Greece/Athens/Athens01.jpg",
                            Alt="Athens image 1"
                        },
                        new DestinationImage
                        {
                            Src="Greece/Athens/Athens02.jpg",
                            Alt="Athens image 2"
                        },
                        new DestinationImage
                        {
                            Src="Greece/Athens/Athens03.jpg",
                            Alt="Athens image 3"
                        },
                    }
                },
                //Greece -> Santorini
                new Destination
                {
                    DestinationName="Santorini",
                    DestinationDescription="An idyllic island with white-washed buildings, blue-domed churches, and stunning sunsets over the Aegean Sea.",
                    Country=country[6],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Greece/Santorini/Santorini01.jpg",
                            Alt="Santorini image 1"
                        },
                        new DestinationImage
                        {
                            Src="Greece/Santorini/Santorini02.jpg",
                            Alt="Santorini image 2"
                        },
                        new DestinationImage
                        {
                            Src="Greece/Santorini/Santorini03.jpg",
                            Alt="Santorini image 3"
                        },
                    }
                },
                //Greece -> Crete
                new Destination
                {
                    DestinationName="Crete",
                    DestinationDescription="Historic charm, stunning beaches, and Mediterranean allure. Explore ancient sites, savor local cuisine, and embrace warm hospitality. Unforgettable experiences await on this enchanting island.",
                    Country=country[6],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Greece/Crete/Crete01.jpg",
                            Alt="Crete image 1"
                        },
                        new DestinationImage
                        {
                            Src="Greece/Crete/Crete02.jpg",
                            Alt="Crete image 2"
                        },
                        new DestinationImage
                        {
                            Src="Greece/Crete/Crete03.jpg",
                            Alt="Crete image 3"
                        },
                    }
                },
                //Mexico -> Mexico City
                new Destination
                {
                    DestinationName="Mexico City",
                    DestinationDescription="The bustling capital with historical sites like the Zócalo, ancient Teotihuacan pyramids, and vibrant neighborhoods.",
                    Country=country[7],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Mexico/MexicoCity/MexicoCity01.jpg",
                            Alt="Mexico City image 1"
                        },
                        new DestinationImage
                        {
                            Src="Mexico/MexicoCity/MexicoCity02.jpg",
                            Alt="Mexico City image 2"
                        },
                        new DestinationImage
                        {
                            Src="Mexico/MexicoCity/MexicoCity03.jpg",
                            Alt="Mexico City image 3"
                        },
                    }
                },
                //Mexico -> Tulum
                new Destination
                {
                    DestinationName="Tulum",
                    DestinationDescription="A coastal paradise with well-preserved Mayan ruins overlooking the Caribbean Sea.",
                    Country=country[7],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Mexico/Tulum/Tulum01.jpg",
                            Alt="Tulum image 1"
                        },
                        new DestinationImage
                        {
                            Src="Mexico/Tulum/Tulum02.jpg",
                            Alt="Tulum image 2"
                        },
                        new DestinationImage
                        {
                            Src="Mexico/Tulum/Tulum03.jpg",
                            Alt="Tulum image 3"
                        },
                    }
                },
                //Mexico -> Cancun
                new Destination
                {
                    DestinationName="Cancun",
                    DestinationDescription="Famous for its pristine beaches, vibrant nightlife, and proximity to the ancient Mayan archaeological sites.",
                    Country=country[7],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Mexico/Cancun/Cancun01.jpg",
                            Alt="Cancun image 1"
                        },
                        new DestinationImage
                        {
                            Src="Mexico/Cancun/Cancun02.jpg",
                            Alt="Cancun image 2"
                        },
                        new DestinationImage
                        {
                            Src="Mexico/Cancun/Cancun03.jpg",
                            Alt="Cancun image 3"
                        },
                    }
                },
                //Russia -> Saint Petersburg
                new Destination
                {
                    DestinationName="Saint Petersburg",
                    DestinationDescription="A cultural hub with the State Hermitage Museum, Winter Palace, and the grandeur of the Peter and Paul Fortress.",
                    Country=country[8],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Russia/SaintPetersburg/SaintPetersburg01.jpg",
                            Alt="Saint Petersburg image 1"
                        },
                        new DestinationImage
                        {
                            Src="Russia/SaintPetersburg/SaintPetersburg02.jpg",
                            Alt="Saint Petersburg image 2"
                        },
                        new DestinationImage
                        {
                            Src="Russia/SaintPetersburg/SaintPetersburg03.jpg",
                            Alt="Saint Petersburg image 3"
                        }
                    }
                },
                //Russia -> Moscow
                new Destination
                {
                    DestinationName="Moscow",
                    DestinationDescription="The capital showcases the Red Square, Kremlin, and the iconic St. Basil's Cathedral.",
                    Country=country[8],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Russia/Moscow/Moscow01.jpg",
                            Alt="Moscow image 1"
                        },
                        new DestinationImage
                        {
                            Src="Russia/Moscow/Moscow02.jpg",
                            Alt="Moscow image 2"
                        },
                        new DestinationImage
                        {
                            Src="Russia/Moscow/Moscow03.jpg",
                            Alt="Moscow image 3"
                        },
                    }
                },
                //Russia -> Lake Baikal
                new Destination
                {
                    DestinationName="Lake Baikal",
                    DestinationDescription="The deepest freshwater lake on Earth, surrounded by Siberian wilderness and unique biodiversity.",
                    Country= country[8],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Russia/LakeBaikal/LakeBaikal01.jpg",
                            Alt="Lake Baikal image 1"
                        },
                        new DestinationImage
                        {
                            Src="Russia/LakeBaikal/LakeBaikal02.jpg",
                            Alt="Lake Baikal image 2"
                        },
                        new DestinationImage
                        {
                            Src="Russia/LakeBaikal/LakeBaikal03.jpg",
                            Alt="Lake Baikal image 3"
                        },
                    }
                },
                //India -> New Delhi
                new Destination
                {
                    DestinationName="New Delhi",
                    DestinationDescription="India's capital featuring the historic Red Fort, India Gate, and the Lotus Temple.",
                    Country=country[9],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="India/NewDelhi/NewDelhi01.jpg",
                            Alt="New Delhi image 1"
                        },
                        new DestinationImage
                        {
                            Src="India/NewDelhi/NewDelhi02.jpg",
                            Alt="New Delhi image 2"
                        },
                        new DestinationImage
                        {
                            Src="India/NewDelhi/NewDelhi03.jpg",
                            Alt="New Delhi image 3"
                        },
                    }
                },
                //India -> Agra
                new Destination
                {
                    DestinationName="Agra",
                    DestinationDescription="Agra, renowned for the iconic Taj Mahal, a UNESCO World Heritage site and symbol of eternal love. Beyond the mesmerizing marble mausoleum, Agra Fort stands as a testament to Mughal architecture, making Agra a captivating destination for history and architecture enthusiasts.",
                    Country=country[9],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="India/Agra/Agra01.jpg",
                            Alt="Agra image 1"
                        },
                        new DestinationImage
                        {
                            Src="India/Agra/Agra02.jpg",
                            Alt="Agra image 2"
                        },
                        new DestinationImage
                        {
                            Src="India/Agra/Agra03.jpg",
                            Alt="Agra image 3"
                        },
                    }
                },
                //India -> Varanasi
                new Destination
                {
                    DestinationName="Varanasi",
                    DestinationDescription="The spiritual heart of India, situated on the banks of the Ganges River, known for ghats, temples, and religious rituals.",
                    Country=country[9],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="India/Varanasi/Varanasi01.jpg",
                            Alt="Varanasi image 1"
                        },
                        new DestinationImage
                        {
                            Src="India/Varanasi/Varanasi02.jpg",
                            Alt="Varanasi image 2"
                        },
                        new DestinationImage
                        {
                            Src="India/Varanasi/Varanasi03.jpg",
                            Alt="Varanasi image 3"
                        },
                    }
                },
                //Peru -> Lima
                new Destination
                {
                    DestinationName="Lima",
                    DestinationDescription="Peru's capital with a mix of colonial and modern architecture, vibrant markets, and the historic Plaza de Armas.",
                    Country=country[10],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Peru/Lima/Lima01.jpg",
                            Alt="Lima image 1"
                        },
                        new DestinationImage
                        {
                            Src="Peru/Lima/Lima02.jpg",
                            Alt="Lima image 2"
                        },
                        new DestinationImage
                        {
                            Src="Peru/Lima/Lima03.jpg",
                            Alt="Lima image 3"
                        },
                    }
                },
                //Peru -> Machu Picchu
                new Destination
                {
                    DestinationName="Machu Picchu",
                    DestinationDescription="The ancient Incan citadel, perched high in the Andes, is a UNESCO site and one of the New Seven Wonders of the World.",
                    Country=country[10],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Peru/MachuPicchu/MachuPicchu01.jpg",
                            Alt="Machu Picchu image 1"
                        },
                        new DestinationImage
                        {
                            Src="Peru/MachuPicchu/MachuPicchu02.jpg",
                            Alt="Machu Picchu image 2"
                        },
                        new DestinationImage
                        {
                            Src="Peru/MachuPicchu/MachuPicchu03.jpg",
                            Alt="Machu Picchu image 3"
                        },
                    }
                },
                //Peru -> Cusco
                new Destination
                {
                    DestinationName="Cusco",
                    DestinationDescription="Situated in the Andes Mountains, Cusco, once the capital of the Inca Empire, stands as a city rich in history. The architecture reflects a harmonious blend of Inca and Spanish colonial influences. The heart of Cusco is adorned with historical gems such as the Plaza de Armas and the Cathedral of Santo Domingo. Cusco serves as the gateway to the iconic Machu Picchu, drawing visitors with its archaeological marvels. The city's cultural vibrancy, bustling markets, and traditional festivals contribute to its unique charm. Exploring the cobblestone streets, travelers encounter the essence of Inca heritage and the imprints of Spanish conquest.",
                    Country=country[10],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Peru/Cusco/Cusco01.jpg",
                            Alt="Cusco image 1"
                        },
                        new DestinationImage
                        {
                            Src="Peru/Cusco/Cusco02.jpg",
                            Alt="Cusco image 2"
                        },
                        new DestinationImage
                        {
                            Src="Peru/Cusco/Cusco03.jpg",
                            Alt="Cusco image 3"
                        },
                    }
                },
                //South Korea -> Seoul
                new Destination
                {
                    DestinationName="Seoul",
                    DestinationDescription="The dynamic capital, blending modern skyscrapers with ancient palaces like Gyeongbokgung and vibrant street markets.",
                    Country=country[11],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="SouthKorea/Seoul/Seoul01.jpg",
                            Alt="Seoul image 1"
                        },
                        new DestinationImage
                        {
                            Src="SouthKorea/Seoul/Seoul02.jpg",
                            Alt="Seoul image 2"
                        },
                        new DestinationImage
                        {
                            Src="SouthKorea/Seoul/Seoul03.jpg",
                            Alt="Seoul image 3"
                        },
                    }
                },
                //South Korea -> Jeju Island
                new Destination
                {
                    DestinationName="Jeju Island",
                    DestinationDescription="A volcanic island known for waterfalls, lava tubes, and the iconic Hallasan Mountain.",
                    Country=country[11],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="SouthKorea/JejuIsland/JejuIsland01.jpg",
                            Alt="Jeju Island image 1"
                        },
                        new DestinationImage
                        {
                            Src="SouthKorea/JejuIsland/JejuIsland02.jpg",
                            Alt="Jeju Island image 2"
                        },
                        new DestinationImage
                        {
                            Src="SouthKorea/JejuIsland/JejuIsland03.jpg",
                            Alt="Jeju Island image 3"
                        },
                    }
                },
                //South Korea -> Busan
                new Destination
                {
                    DestinationName="Busan",
                    DestinationDescription="A coastal city with picturesque beaches, the bustling Jagalchi Fish Market, and the scenic Gamcheon Culture Village.",
                    Country=country[11],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="SouthKorea/Busan/Busan01.jpg",
                            Alt="Busan image 1"
                        },
                        new DestinationImage
                        {
                            Src="SouthKorea/Busan/Busan02.jpg",
                            Alt="Busan image 2"
                        },
                        new DestinationImage
                        {
                            Src="SouthKorea/Busan/Busan03.jpg",
                            Alt="Busan image 3"
                        },
                    }
                },
                //Morocco -> Marrakech
                new Destination
                {
                    DestinationName="Marrakech",
                    DestinationDescription="A city known for its vibrant souks, the historic medina, and iconic landmarks like the Jardin Majorelle and Koutoubia Mosque.",
                    Country=country[12],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Morocco/Marrakech/Marrakech01.jpg",
                            Alt="Marrakech image 1"
                        },
                        new DestinationImage
                        {
                            Src="Morocco/Marrakech/Marrakech02.jpg",
                            Alt="Marrakech image 2"
                        },
                        new DestinationImage
                        {
                            Src="Morocco/Marrakech/Marrakech03.jpg",
                            Alt="Marrakech image 3"
                        },
                    }
                },
                //Morocco -> Fes
                new Destination
                {
                    DestinationName="Fes",
                    DestinationDescription="Morocco's cultural and spiritual center, featuring the ancient medina, Bou Inania Madrasa, and the tanneries.",
                    Country=country[12],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Morocco/Fes/Fes01.jpg",
                            Alt="Fes image 1"
                        },
                        new DestinationImage
                        {
                            Src="Morocco/Fes/Fes02.jpg",
                            Alt="Fes image 2"
                        },
                        new DestinationImage
                        {
                            Src="Morocco/Fes/Fes03.jpg",
                            Alt="Fes image 3"
                        },
                    }
                },
                //Morocco -> Casablanca
                new Destination
                {
                    DestinationName="Casablanca",
                    DestinationDescription="Casablanca, Morocco's largest city, seamlessly blends modernity with tradition. Explore the iconic Hassan II Mosque, wander through the bustling medina, and experience the vibrant cultural tapestry of this coastal metropolis.",
                    Country=country[12],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Morocco/Casablanca/Casablanca01.jpg",
                            Alt="Casablanca image 1"
                        },
                        new DestinationImage
                        {
                            Src="Morocco/Casablanca/Casablanca02.jpg",
                            Alt="Casablanca image 2"
                        },
                        new DestinationImage
                        {
                            Src="Morocco/Casablanca/Casablanca03.jpg",
                            Alt="Casablanca image 3"
                        }
                    }
                },
                //Argentina -> Buenos Aires
                new Destination
                {
                    DestinationName="Buenos Aires",
                    DestinationDescription="Buenos Aires is a cosmopolitan city known for its passionate tango, vibrant art scene, and eclectic architecture.",
                    Country=country[13],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Argentina/BuenosAires/BuenosAires01.jpg",
                            Alt="Buenos Aires image 1"
                        },
                        new DestinationImage
                        {
                            Src="Argentina/BuenosAires/BuenosAires02.jpg",
                            Alt="Buenos Aires image 2"
                        },
                        new DestinationImage
                        {
                            Src="Argentina/BuenosAires/BuenosAires03.jpg",
                            Alt="Buenos Aires image 3"
                        },
                    }
                },
                //Argentina -> Mendoza
                new Destination
                {
                    DestinationName="Mendoza",
                    DestinationDescription="Mendoza, nestled in the foothills of the Andes, is Argentina's wine country par excellence. Famous for its Malbec wines, the region offers picturesque vineyards, stunning mountain landscapes, and outdoor activities.",
                    Country=country[13],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Argentina/Mendoza/Mendoza01.jpg",
                            Alt="Mendoza image 1"
                        },
                        new DestinationImage
                        {
                            Src="Argentina/Mendoza/Mendoza02.jpg",
                            Alt="Mendoza image 2"
                        },
                        new DestinationImage
                        {
                            Src="Argentina/Mendoza/Mendoza03.jpg",
                            Alt="Mendoza image 3"
                        }
                    }
                },
                //Argentina -> Iguazu Falls
                new Destination
                {
                    DestinationName="Iguazu Falls",
                    DestinationDescription="Shared with Brazil, Iguazu Falls is a UNESCO World Heritage site, featuring breathtaking waterfalls amidst lush rainforests, providing a mesmerizing natural spectacle.",
                    Country=country[13],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Argentina/IguazuFalls/IguazuFalls01.jpg",
                            Alt="Iguazu Falls image 1"
                        },
                        new DestinationImage
                        {
                            Src="Argentina/IguazuFalls/IguazuFalls02.jpg",
                            Alt="Iguazu Falls image 2"
                        },
                        new DestinationImage
                        {
                            Src="Argentina/IguazuFalls/IguazuFalls03.jpg",
                            Alt="Iguazu Falls image 3"
                        },
                    }
                },
                //Tanzania -> Kilimanjaro
                new Destination
                {
                    DestinationName="Kilimanjaro",
                    DestinationDescription="Kilimanjaro, Africa's highest peak, offers trekking adventures through lush rainforests, alpine meadows, and glaciers. The climate varies dramatically as you ascend, providing a unique and diverse experience.",
                    Country=country[14],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Tanzania/Kilimanjaro/Kilimanjaro01.jpg",
                            Alt="Kilimanjaro image 1"
                        },
                        new DestinationImage
                        {
                            Src="Tanzania/Kilimanjaro/Kilimanjaro02.jpg",
                            Alt="Kilimanjaro image 2"
                        },
                        new DestinationImage
                        {
                            Src="Tanzania/Kilimanjaro/Kilimanjaro03.jpg",
                            Alt="Kilimanjaro image 3"
                        },
                    }
                },
                //Tanzania -> Zanzibar
                new Destination
                {
                    DestinationName="Zanzibar",
                    DestinationDescription="Zanzibar is a tropical paradise renowned for spice farms, the historic Stone Town's cultural richness, and pristine beaches with turquoise waters. The climate creates an ideal setting for year-round relaxation.",
                    Country=country[14],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Tanzania/Zanzibar/Zanzibar01.jpg",
                            Alt="Zanzibar image 1"
                        },
                        new DestinationImage
                        {
                            Src="Tanzania/Zanzibar/Zanzibar02.jpg",
                            Alt="Zanzibar image 2"
                        },
                        new DestinationImage
                        {
                            Src="Tanzania/Zanzibar/Zanzibar03.jpg",
                            Alt="Zanzibar image 3"
                        },
                    }
                },
                //Tanzania -> Serengeti National Park
                new Destination
                {
                    DestinationName="Serengeti National Park",
                    DestinationDescription="Famous for the annual wildebeest migration, Serengeti National Park is a wildlife haven with vast savannas teeming with diverse and abundant animal species. The climate plays a crucial role in supporting this unique ecosystem.",
                    Country=country[14],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Tanzania/SerengetiNationalPark/SerengetiNationalPark01.jpg",
                            Alt="Serengeti National Park image 1"
                        },
                        new DestinationImage
                        {
                            Src="Tanzania/SerengetiNationalPark/SerengetiNationalPark02.jpg",
                            Alt="Serengeti National Park image 2"
                        },
                        new DestinationImage
                        {
                            Src="Tanzania/SerengetiNationalPark/SerengetiNationalPark03.jpg",
                            Alt="Serengeti National Park image 3"
                        },
                    }
                },
                //Australia -> Sydney
                new Destination
                {
                    DestinationName="Sydney",
                    DestinationDescription="Sydney, Australia's iconic harbor city, is a vibrant metropolis known for the Sydney Opera House and the Sydney Harbour Bridge. Its stunning beaches, including Bondi and Manly, offer sun-soaked relaxation.",
                    Country=country[15],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Australia/Sydney/Sydney01.jpg",
                            Alt="Sydney image 1"
                        },
                        new DestinationImage
                        {
                            Src="Australia/Sydney/Sydney02.jpg",
                            Alt="Sydney image 2"
                        },
                        new DestinationImage
                        {
                            Src="Australia/Sydney/Sydney03.jpg",
                            Alt="Sydney image 3"
                        },
                    }
                },
                //Australia -> Melbourne
                new Destination
                {
                    DestinationName="Melbourne",
                    DestinationDescription="Melbourne, a cultural hub, captivates with its diverse arts, coffee culture, and eclectic neighborhoods. Renowned for its street art, Melbourne hosts events like the Australian Open and the Melbourne International Comedy Festival.",
                    Country=country[15],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Australia/Melbourne/Melbourne01.jpg",
                            Alt="Melbourne image 1"
                        },
                        new DestinationImage
                        {
                            Src="Australia/Melbourne/Melbourne02.jpg",
                            Alt="Melbourne image 2"
                        },
                        new DestinationImage
                        {
                            Src="Australia/Melbourne/Melbourne03.jpg",
                            Alt="Melbourne image 3"
                        },
                    }
                },
                //Australia -> Tasmania
                new Destination
                {
                    DestinationName="Tasmania",
                    DestinationDescription="Tasmania, an island state, is a heaven for nature lovers. With pristine wilderness, including the iconic Cradle Mountain, and rugged coastlines, it offers outdoor adventures.",
                    Country=country[15],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Australia/Tasmania/Tasmania01.jpg",
                            Alt="Tasmania image 1"
                        },
                        new DestinationImage
                        {
                            Src="Australia/Tasmania/Tasmania02.jpg",
                            Alt="Tasmania image 2"
                        },
                        new DestinationImage
                        {
                            Src="Australia/Tasmania/Tasmania03.jpg",
                            Alt="Tasmania image 3"
                        }
                    }
                },
                //Antarctica -> Antarctica
                new Destination
                {
                    DestinationName="Antarctica",
                    DestinationDescription="Antarctica, Earth's southernmost continent, is a vast, icy wilderness known for extreme cold, towering glaciers, and unique wildlife such as penguins and seals. With no permanent population, it is primarily dedicated to scientific research and environmental preservation under the Antarctic Treaty. Visits are limited, and travel is usually organized through specialized tours or research expeditions. Antarctica offers a pristine and breathtaking landscape for those seeking unparalleled natural beauty.",
                    Country=country[16],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Antarctica/Antarctica/Antarctica01.jpg",
                            Alt="Antarctica image 1"
                        },
                        new DestinationImage
                        {
                            Src="Antarctica/Antarctica/Antarctica02.jpg",
                            Alt="Antarctica image 2"
                        },
                        new DestinationImage
                        {
                            Src="Antarctica/Antarctica/Antarctica03.jpg",
                            Alt="Antarctica image 3"
                        },
                    }
                },
                //Spain -> Barcelona
                new Destination
                {
                    DestinationName="Barcelona",
                    DestinationDescription="A dynamic city with impressive architecture by Antoni Gaudí, beaches, and a rich cultural scene.",
                    Country=country[17],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Spain/Barcelona/Barcelona01.jpg",
                            Alt="Barcelona image 1"
                        },
                        new DestinationImage
                        {
                            Src="Spain/Barcelona/Barcelona02.jpg",
                            Alt="Barcelona image 2"
                        },
                        new DestinationImage
                        {
                            Src="Spain/Barcelona/Barcelona03.jpg",
                            Alt="Barcelona image 3"
                        }
                    }
                },
                //Spain -> Madrid
                new Destination
                {
                    DestinationName="Madrid",
                    DestinationDescription="The capital city known for its historical landmarks, museums, and vibrant nightlife.",
                    Country=country[17],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Spain/Madrid/Madrid01.jpg",
                            Alt="Madrid image 1"
                        },
                        new DestinationImage
                        {
                            Src="Spain/Madrid/Madrid02.jpg",
                            Alt="Madrid image 2"
                        },
                        new DestinationImage
                        {
                            Src="Spain/Madrid/Madrid03.jpg",
                            Alt="Madrid image 3"
                        }
                    }
                },
                //Spain -> Seville
                new Destination
                {
                    DestinationName="Seville",
                    DestinationDescription="Famous for its Moorish architecture, flamenco dancing, and historical sites like the Alcazar.",
                    Country=country[17],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Spain/Seville/Seville01.jpg",
                            Alt="Seville image 1"
                        },
                        new DestinationImage
                        {
                            Src="Spain/Seville/Seville02.jpg",
                            Alt="Seville image 2"
                        },
                        new DestinationImage
                        {
                            Src="Spain/Seville/Seville03.jpg",
                            Alt="Seville image 3"
                        }
                    }
                },
                //Italy -> Rome
                new Destination
                {
                    DestinationName="Rome",
                    DestinationDescription="The capital city known for its ancient ruins, art, and the Vatican City.",
                    Country=country[18],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Italy/Rome/Rome01.jpg",
                            Alt="Rome image 1"
                        },
                        new DestinationImage
                        {
                            Src="Italy/Rome/Rome02.jpg",
                            Alt="Rome image 2"
                        },
                        new DestinationImage
                        {
                            Src="Italy/Rome/Rome03.jpg",
                            Alt="Rome image 3"
                        }
                    }
                },
                //Italy -> Venice
                new Destination
                {
                    DestinationName="Venice",
                    DestinationDescription="Famous for its canals, historic architecture, and art, with attractions like St. Mark's Basilica.",
                    Country=country[18],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Italy/Venice/Venice01.jpg",
                            Alt="Venice image 1"
                        },
                        new DestinationImage
                        {
                            Src="Italy/Venice/Venice02.jpg",
                            Alt="Venice image 2"
                        },
                        new DestinationImage
                        {
                            Src="Italy/Venice/Venice03.jpg",
                            Alt="Venice image 3"
                        }
                    }
                },
                //Italy -> Florence
                new Destination
                {
                    DestinationName="Florence",
                    DestinationDescription="An art-centric city with masterpieces from the Renaissance, beautiful bridges, and palaces.",
                    Country=country[18],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Italy/Florence/Florence01.jpg",
                            Alt="Florence image 1"
                        },
                        new DestinationImage
                        {
                            Src="Italy/Florence/Florence02.jpg",
                            Alt="Florence image 2"
                        },
                        new DestinationImage
                        {
                            Src="Italy/Florence/Florence03.jpg",
                            Alt="Florence image 3"
                        }
                    }
                },
                //Switzerland -> Lucerne
                new Destination
                {
                    DestinationName="Lucerne",
                    DestinationDescription="A picturesque city on the shores of Lake Lucerne, known for its medieval architecture and the Chapel Bridge.",
                    Country=country[19],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Switzerland/Lucerne/Lucerne01.jpg",
                            Alt="Lucerne image 1"
                        },
                        new DestinationImage
                        {
                            Src="Switzerland/Lucerne/Lucerne02.jpg",
                            Alt="Lucerne image 2"
                        },
                        new DestinationImage
                        {
                            Src="Switzerland/Lucerne/Lucerne03.jpg",
                            Alt="Lucerne image 3"
                        }
                    }
                },
                //Switzerland -> Zermatt
                new Destination
                {
                    DestinationName="Zermatt",
                    DestinationDescription="A car-free village known for the Matterhorn, skiing, and hiking.",
                    Country=country[19],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Switzerland/Zermatt/Zermatt01.jpg",
                            Alt="Zermatt image 1"
                        },
                        new DestinationImage
                        {
                            Src="Switzerland/Zermatt/Zermatt02.jpg",
                            Alt="Zermatt image 2"
                        },
                        new DestinationImage
                        {
                            Src="Switzerland/Zermatt/Zermatt03.jpg",
                            Alt="Zermatt image 3"
                        }
                    }
                },
                //Switzerland -> Interlaken
                new Destination
                {
                    DestinationName="Interlaken",
                    DestinationDescription="Nestled between Lake Thun and Lake Brienz, surrounded by the Alps, known for adventurous activities and stunning landscapes.",
                    Country=country[19],
                    IsActive=true,
                    Images=new List<DestinationImage>
                    {
                        new DestinationImage
                        {
                            Src="Switzerland/Interlaken/Interlaken01.jpg",
                            Alt="Interlaken image 1"
                        },
                        new DestinationImage
                        {
                            Src="Switzerland/Interlaken/Interlaken02.jpg",
                            Alt="Interlaken image 2"
                        },
                        new DestinationImage
                        {
                            Src="Switzerland/Interlaken/Interlaken03.jpg",
                            Alt="Interlaken image 3"
                        }
                    }
                },
            };

            var climates = new List<Climate>
            {
                //Dominican Republic -> Punta Cana
                new Climate
                {
                    Destination=destinations[3],
                    ClimateInfo="Punta Cana features a tropical wet and dry climate under the Köppen climate classification. The weather is fairly consistent all year, with an average temperature of 26 °C (79 °F). The hot and humid season lasts from May to October, and during the day temperatures might reach 35 °C (95 °F). From November to March, temperatures during the evening are around 20 °C (68 °F)."
                },
                //Cuba -> Trinidad
                new Climate
                {
                    Destination=destinations[2],
                    ClimateInfo="Temperatures are pleasant through the whole year. Average temperatures range from 26°C (january) to 31°C (june). The rainiest months are: august, may and june. Best time to visit Trinidad is: january, february, march, april and december."
                },
                //Cuba -> Varadero
                new Climate
                {
                    Destination=destinations[1],
                    ClimateInfo="The climate in Varadero is tropical, for it is at sea level at the tip of the island. Weather may change due to many factors such as exposure to hurricanes, windy tropical storms and by the cooling effect of the trade winds. It is surrounded by enormous bodies of water that can cool air flowing from North America. The yearly mean is 25˚ Celsius (77 ˚F). Summer mean is 27˚Celsius (80.6 ˚F), while winter mean is 21˚C (69.8 ˚F). Humidity is 81% and the average yearly rainfall is roughly 1,400 millimeters (55 inches). June 1 is the beginning of hurricane season; this normally ends on November 15."
                },
                //Cuba -> Havana
                new Climate
                {
                    Destination=destinations[0],
                    ClimateInfo="Havana has a tropical savanna climate (Aw) that closely borders on a tropical rainforest climate (Af) and a tropical monsoon climate (Am). Average temperatures range from 22 °C (72 °F) in January and February to 28 °C (82 °F) in August. Rainfall is heaviest in June and October and lightest from December through April."
                },
                //Egypt -> Cairo
                new Climate
                {
                    Destination=destinations[4],
                    ClimateInfo="In Cairo, the summers are long, hot, humid, arid, and clear and the winters are cool, dry, and mostly clear. Over the course of the year, the temperature typically varies from 10°C to 35°C and is rarely below 8°C or above 39°C. The best times of year to visit Cairo for hot-weather activities are from mid May to mid July and from mid August to late October."
                },
                //Dominican Republic -> Santo Domingo
                new Climate
                {
                    Destination=destinations[5],
                    ClimateInfo="Santo Domingo experiences a tropical climate with high temperatures year-round. The average annual temperature ranges from 72°F to 88°F (22°C to 31°C). The city sees a distinct wet season from May to October, with the possibility of hurricanes."
                },
                //Dominican Republic -> Saona Island
                new Climate
                {
                    Destination=destinations[6],
                    ClimateInfo="Saona Island shares the tropical climate of the Dominican Republic. Visitors can expect warm temperatures throughout the year, with an average annual temperature range of 77°F to 88°F (25°C to 31°C). The dry season is the ideal time to enjoy the pristine beaches and outdoor activities."
                },
                //Egypt -> Luxor
                new Climate
                {
                    Destination=destinations[7],
                    ClimateInfo="Luxor, located in the desert, has an arid climate. Summers are extremely hot, with temperatures exceeding 104°F (40°C), while winters are mild, ranging from 41°F to 73°F (5°C to 23°C). Rainfall is scarce."
                },
                //Egypt -> Sharm El Sheikh
                new Climate
                {
                    Destination=destinations[8],
                    ClimateInfo="Sharm El Sheikh has a desert climate characterized by high temperatures and low humidity. Summers are hot, with temperatures often exceeding 104°F (40°C), while winters are mild, ranging from 59°F to 77°F (15°C to 25°C). Rainfall is minimal. The Red Sea provides a cooling effect, making it a popular year-round destination for water activities."
                },
                //France -> Paris
                new Climate
                {
                    Destination=destinations[9],
                    ClimateInfo="The climate in Paris is oceanic, with warm summers and cool winters. Summers (July, around 25°C) are pleasant, and winters (January, around 5°C) are chilly."
                },
                //France -> Nice
                new Climate
                {
                    Destination=destinations[10],
                    ClimateInfo="Nice experiences a Mediterranean climate with hot, dry summers (July, around 30°C) and mild, wet winters (January, around 10°C)."
                },
                //France -> Avignon
                new Climate
                {
                    Destination=destinations[11],
                    ClimateInfo="Avignon also has a Mediterranean climate, characterized by hot summers (July, around 30°C) and mild winters (January, around 5°C)."
                },
                //Turkey -> Istanbul
                new Climate
                {
                    Destination=destinations[12],
                    ClimateInfo="Istanbul has a humid subtropical climate with hot summers (July, around 30°C) and cool, wet winters (January, around 5°C)."
                },
                //Turkey -> Cappadocia
                new Climate
                {
                    Destination=destinations[13],
                    ClimateInfo="Cappadocia has a continental climate with hot summers (July, around 30°C) and cold winters (January, around 0°C)."
                },
                //Turkey -> Antalya
                new Climate
                {
                    Destination=destinations[14],
                    ClimateInfo="Antalya experiences a Mediterranean climate with hot, dry summers (July, around 35°C) and mild, wet winters (January, around 10°C)."
                },
                //Brazil -> Rio De Janeiro
                new Climate
                {
                    Destination=destinations[15],
                    ClimateInfo="Rio has a tropical climate with hot, humid summers (January, around 30°C) and mild, dry winters (July, around 20°C)."
                },
                //Brazil -> Florianopolis
                new Climate
                {
                    Destination=destinations[16],
                    ClimateInfo="Subtropical climate, characterized by warm summers (26°C/79°F) and mild winters (18°C/64°F), occasionally accompanied by rainfall."
                },
                //Brazil -> Salvador
                new Climate
                {
                    Destination=destinations[17],
                    ClimateInfo="Salvador has a tropical climate with a wet season from April to July and a dry season from October to March. Temperatures average between 23 to 30°C (73 to 86°F) year-round."
                },
                //Greece -> Athens
                new Climate
                {
                    Destination=destinations[18],
                    ClimateInfo="Athens has a Mediterranean climate with hot, dry summers (July, around 33°C) and mild, wet winters (January, around 10°C)."
                },
                //Greece -> Santorini
                new Climate
                {
                    Destination=destinations[19],
                    ClimateInfo="Santorini also has a Mediterranean climate, with hot, dry summers (July, around 29°C) and mild, rainy winters (January, around 12°C)."
                },
                //Greece -> Crete
                new Climate
                {
                    Destination=destinations[20],
                    ClimateInfo="A captivating island with a Mediterranean climate. Hot, dry summers (June-September, 25-35°C) and mild, wet winters (December-February, 8-16°C). "
                },
                //Mexico -> Mexico City
                new Climate
                {
                    Destination=destinations[21],
                    ClimateInfo="Mexico City has a subtropical highland climate with mild, dry winters (January, around 15°C) and warmer, rainy summers (July, around 20°C)."
                },
                //Mexico -> Tulum
                new Climate
                {
                    Destination=destinations[22],
                    ClimateInfo="Tulum experiences a tropical climate with warm temperatures throughout the year. Summers are hot and humid, with temperatures around 28°C."
                },
                //Mexico -> Cancun
                new Climate
                {
                    Destination=destinations[23],
                    ClimateInfo="Cancun has a tropical climate with hot summers (July, around 32°C) and warm winters (January, around 23°C)."
                },
                //Russia -> Saint Petersburg
                new Climate
                {
                    Destination=destinations[24],
                    ClimateInfo="Saint Petersburg has a humid continental climate with cool summers (July, around 18°C) and cold winters (January, around -6°C)."
                },
                //Russia -> Moscow
                new Climate
                {
                    Destination=destinations[25],
                    ClimateInfo="Moscow also has a humid continental climate with warm summers (July, around 19°C) and cold winters (January, around -9°C)."
                },
                //Russia -> Lake Baikal
                new Climate
                {
                    Destination=destinations[26],
                    ClimateInfo="The climate around Lake Baikal is characterized by cold winters (January, around -19°C) and cool summers (July, around 12°C)."
                },
                //India -> New Delhi
                new Climate
                {
                    Destination=destinations[27],
                    ClimateInfo="New Delhi has a hot semi-arid climate with extremely hot summers (May, around 38°C) and cool winters (January, around 12°C)."
                },
                //India -> Agra
                new Climate
                {
                    Destination=destinations[28],
                    ClimateInfo="Agra experiences a hot semi-arid climate with scorching summers, where temperatures can soar above 40°C (104°F). Winters are cool and foggy, with temperatures ranging from 8°C to 14°C (46°F to 57°F)."
                },
                //India -> Varanasi
                new Climate
                {
                    Destination=destinations[29],
                    ClimateInfo="Varanasi has a humid subtropical climate with hot summers (May, around 38°C) and cool winters (January, around 12°C)."
                },
                //Peru -> Lima
                new Climate
                {
                    Destination=destinations[30],
                    ClimateInfo="Lima has a subtropical desert climate with warm temperatures throughout the year. Summers (February, around 27°C) are hot and dry."
                },
                //Peru -> Machu Picchu
                new Climate
                {
                    Destination=destinations[31],
                    ClimateInfo="The climate around Machu Picchu is tropical mountainous, with warm days (November, around 22°C) and cooler nights."
                },
                //Peru -> Cusco
                new Climate
                {
                    Destination=destinations[32],
                    ClimateInfo="Highland climate, dry season (April-October), clear skies, daytime temperatures 15-20°C (59-68°F); wet season (November-March), occasional showers, daytime temperatures 12-18°C (54-64°F)."
                },
                //South Korea -> Seoul
                new Climate
                {
                    Destination=destinations[33],
                    ClimateInfo="Seoul has a humid continental climate with hot, humid summers (August, around 27°C) and cold, dry winters (January, around -6°C)."
                },
                //South Korea -> Jeju Island
                new Climate
                {
                    Destination=destinations[34],
                    ClimateInfo="Jeju Island has a humid subtropical climate with warm temperatures throughout the year. Summers (August, around 29°C) are humid."
                },
                //South Korea -> Busan
                new Climate
                {
                    Destination=destinations[35],
                    ClimateInfo="Busan experiences a humid subtropical climate with hot, humid summers (August, around 28°C) and mild winters (January, around 3°C)."
                },
                //Morocco -> Marrakech
                new Climate
                {
                    Destination=destinations[36],
                    ClimateInfo="Marrakech has a hot semi-arid climate with extremely hot summers (July, around 38°C) and mild winters (January, around 12°C)."
                },
                //Morocco -> Fes
                new Climate
                {
                    Destination=destinations[37],
                    ClimateInfo="Fes experiences a Mediterranean climate with hot, dry summers (July, around 35°C) and mild, wet winters (January, around 10°C)."
                },
                //Morocco -> Casablanca
                new Climate
                {
                    Destination=destinations[38],
                    ClimateInfo="Casablanca enjoys a Mediterranean climate, with hot summers (20°C to 26°C / 68°F to 79°F) and mild winters (8°C to 17°C / 46°F to 63°F). Refreshing sea breezes contribute to a pleasant atmosphere year-round."
                },
                //Argentina -> Buenos Aires
                new Climate
                {
                    Destination=destinations[39],
                    ClimateInfo="Temperate climate with hot summers (December to February) and mild winters (June to August). Summer averages around 24-33°C (75-91°F), while winter averages around 7-18°C (45-64°F)."
                },
                //Argentina -> Mendoza
                new Climate
                {
                    Destination=destinations[40],
                    ClimateInfo="Mendoza features a desert climate with hot summers and cool winters. Summer temperatures can reach around 32°C (90°F), while winters see daytime temperatures ranging from 14°C to 19°C (57°F to 66°F), making it an ideal year-round destination."
                },
                //Argentina -> Iguazu Falls
                new Climate
                {
                    Destination=destinations[41],
                    ClimateInfo="Subtropical climate with high humidity. Rainfall is evenly distributed throughout the year. Around 18-32°C (64-90°F)."
                },
                //Tanzania -> Kilimanjaro
                new Climate
                {
                    Destination=destinations[42],
                    ClimateInfo="Varied due to different elevations. Lower slopes are tropical, middle zones are temperate, and the summit is icy and cold. Range from 20°C (68°F) at the base to sub-zero temperatures at the summit."
                },
                //Tanzania -> Zanzibar
                new Climate
                {
                    Destination=destinations[43],
                    ClimateInfo="Tropical with a hot season (December to March) and a cooler season (June to October). Around 26-32°C (79-90°F) year-round."
                },
                //Tanzania -> Serengeti National Park
                new Climate
                {
                    Destination=destinations[44],
                    ClimateInfo="Warm and dry (June to October, peak safari season) with a wet season (November to May). Average temperature is 20-30°C (68-86°F) during the day."
                },
                //Australia -> Sydney
                new Climate
                {
                    Destination=destinations[45],
                    ClimateInfo="Temperate climate with warm summers (December to February) and mild winters (June to August). Summer averages around 18-26°C (64-79°F), and winter averages around 8-16°C (46-61°F)."
                },
                //Australia -> Melbourne
                new Climate
                {
                    Destination=destinations[46],
                    ClimateInfo="Temperate climate with cool winters (June to August) and warm summers (December to February). Summer averages around 14-25°C (57-77°F), and winter averages around 6-14°C (43-57°F)."
                },
                //Australia -> Tasmania
                new Climate
                {
                    Destination=destinations[47],
                    ClimateInfo="Temperate maritime climate with cool summers and cold winters. Summer averages around 12-21°C (54-70°F), and winter averages around 3-11°C (37-52°F)."
                },
                //Antartica -> Antarctica
                new Climate
                {
                    Destination=destinations[48],
                    ClimateInfo="Extremely cold with average temperatures ranging from -10°C to -60°C (14°F to -76°F). Strong winds are prevalent, and precipitation is low, making Antarctica the driest continent."
                },
                //Spain -> Barcelona
                new Climate
                {
                    Destination=destinations[49],
                    ClimateInfo="Barcelona has a Mediterranean climate with hot summers (June to September) and mild winters (December to February). The average temperature ranges from 8°C (46°F) in winter to 30°C (86°F) in summer."
                },
                //Spain -> Madrid
                new Climate
                {
                    Destination=destinations[50],
                    ClimateInfo="Madrid has a hot-summer Mediterranean climate with temperatures ranging from 3°C (37°F) in winter to 32°C (90°F) in summer."
                },
                //Spain -> Seville
                new Climate
                {
                    Destination=destinations[51],
                    ClimateInfo="Seville has a hot-summer Mediterranean climate with temperatures ranging from 6°C (43°F) in winter to 35°C (95°F) in summer."
                },
                //Italy -> Rome
                new Climate
                {
                    Destination=destinations[52],
                    ClimateInfo="Rome has a Mediterranean climate with mild, wet winters (average temperature: 2-15°C / 36-59°F) and hot, dry summers (average temperature: 17-32°C / 63-90°F)."
                },
                //Italy -> Venice
                new Climate
                {
                    Destination=destinations[53],
                    ClimateInfo="Venice has a humid subtropical climate with cool winters (average temperature: 0-7°C / 32-45°F) and hot, humid summers (average temperature: 14-26°C / 57-79°F)."
                },
                //Italy -> Florence
                new Climate
                {
                    Destination=destinations[54],
                    ClimateInfo="Florence has a Mediterranean climate with hot summers (average temperature: 17-32°C / 63-90°F) and cool winters (average temperature: 3-15°C / 37-59°F)."
                },
                //Switzerland -> Lucerne
                new Climate
                {
                    Destination=destinations[55],
                    ClimateInfo="Lucerne has a temperate climate with mild temperatures. Winters (average temperature: -1 to 5°C / 30-41°F), summers (average temperature: 12-24°C / 54-75°F)."
                },
                //Switzerland -> Zermatt
                new Climate
                {
                    Destination=destinations[56],
                    ClimateInfo="Zermatt has an alpine climate with cold winters and cool summers. Winters (average temperature: -5 to -1°C / 23 to 30°F), summers (average temperature: 9-18°C / 48-64°F)."
                },
                //Switzerland -> Interlaken
                new Climate
                {
                    Destination=destinations[57],
                    ClimateInfo="Interlaken has a continental climate with cold winters (average temperature: -4 to 4°C / 25-39°F) and cool summers (average temperature: 12-25°C / 54-77°F)."
                },
            };

            var accommodationType = new List<AccommodationType>
            {
                //Hotel
                new AccommodationType
                {
                    AccommodationTypeName="Hotel"
                },
                //Motel
                new AccommodationType
                {
                    AccommodationTypeName="Motel"
                },
                //Homestays
                new AccommodationType
                {
                    AccommodationTypeName="Homestays"
                },
                //Lodges
                new AccommodationType
                {
                    AccommodationTypeName="Lodges"

                }
            };

            var roomTypes = new List<RoomType>
            {
                //Quadruple Room
                new RoomType
                {
                    RoomTypeName="Quadruple Room",
                    Capacity=4
                },
                //Triple Room
                new RoomType
                {
                    RoomTypeName="Triple Room",
                    Capacity=3
                },
                //Double Room
                new RoomType
                {
                    RoomTypeName="Double Room",
                    Capacity=2
                },
                //Master Bed Suite
                new RoomType
                {
                    RoomTypeName="Master Bed Suite",
                    Capacity=2
                }
            };

            var accommodations = new List<Accommodation>
            {
                //Cuba -> Havana
                new Accommodation
                {
                    AccommodationName="Hotel Malecón Views",
                    AccommodationStars=4,
                    Destination=destinations[0],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=2
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=20
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Casa Colonial Oasis",
                    AccommodationStars=3,
                    Destination=destinations[0],
                    AccommodationType=accommodationType[2],
                    Rooms =
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=3
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=2
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=2
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Havana Beach Resort",
                    AccommodationStars=5,
                    Destination=destinations[0],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Cuba -> Varadero
                new Accommodation
                {
                    AccommodationName="Varadero Paradise Hotel",
                    AccommodationStars=5,
                    Destination=destinations[1],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=3
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Sunset Motel Varadero",
                    AccommodationStars=2,
                    Destination=destinations[1],
                    AccommodationType=accommodationType[1],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=3
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=5
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Varadero Seaside Lodge",
                    AccommodationStars=3,
                    Destination=destinations[1],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=2
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=8
                        },
                    }
                },
                //Cuba -> Trinidad
                new Accommodation
                {
                    AccommodationName="Colonial Charm Inn",
                    AccommodationStars=4,
                    Destination=destinations[2],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=18
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Trinidad Heritage Hotel",
                    AccommodationStars=5,
                    Destination=destinations[2],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=12
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Mountain View Lodges Trinidad",
                    AccommodationStars=3,
                    Destination=destinations[2],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=3
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=20
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=6
                        },
                    }
                },
                //Dominican Republic -> Punta Cana
                new Accommodation
                {
                    AccommodationName="Caribbean Paradise Resort",
                    AccommodationStars=5,
                    Destination=destinations[3],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=50
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=25
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Tropical Oasis Lodges",
                    AccommodationStars=3,
                    Destination=destinations[3],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Seaside Bliss Hotel",
                    AccommodationStars=4,
                    Destination=destinations[3],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Egypt -> Cairo
                new Accommodation
                {
                    AccommodationName="Nile View Hotel",
                    AccommodationStars=4,
                    Destination=destinations[4],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Pyramid Grand Resort",
                    AccommodationStars=5,
                    Destination=destinations[4],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Sphinx Palace Homestay",
                    AccommodationStars=3,
                    Destination=destinations[4],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=12
                        },
                    }
                },
                //Dominican Republic -> Santo Domingo
                new Accommodation
                {
                    AccommodationName="Historic Plaza Hotel",
                    AccommodationStars=4,
                    Destination=destinations[5],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=18
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Colonial Elegance Inn",
                    AccommodationStars=3,
                    Destination=destinations[5],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=3
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=20
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=8
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Santo Domingo Beach Resort",
                    AccommodationStars=5,
                    Destination=destinations[5],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=25
                        },
                    }
                },
                //Dominican Republic -> Saona Island
                new Accommodation
                {
                    AccommodationName="Tropical Breeze Resort",
                    AccommodationStars=4,
                    Destination=destinations[6],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=35
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Island Paradise Lodges",
                    AccommodationStars=3,
                    Destination=destinations[6],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=20
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Coral Cove Hotel",
                    AccommodationStars=5,
                    Destination=destinations[6],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=50
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                //Egypt -> Luxor
                new Accommodation
                {
                    AccommodationName="Pharaoh's Palace Hotel",
                    AccommodationStars=5,
                    Destination=destinations[7],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Nile Heritage Resort",
                    AccommodationStars=4,
                    Destination=destinations[7],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=25
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Temple View Homestay",
                    AccommodationStars=3,
                    Destination=destinations[7],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=12
                        },
                    }
                },
                //Egypt -> Sharm El Sheikh
                new Accommodation
                {
                    AccommodationName="Red Sea Paradise Resort",
                    AccommodationStars=5,
                    Destination=destinations[8],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=55
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Coral Divers Lodge",
                    AccommodationStars=4,
                    Destination=destinations[8],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=18
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Golden Sands Hotel",
                    AccommodationStars=3,
                    Destination=destinations[8],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //France -> Paris
                new Accommodation
                {
                    AccommodationName="Eiffel Tower View Hotel",
                    AccommodationStars=4,
                    Destination=destinations[9],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Montmartre Charm Inn",
                    AccommodationStars=3,
                    Destination=destinations[9],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Louvre Grand Hotel",
                    AccommodationStars=5,
                    Destination=destinations[9],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=50
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=25
                        },
                    }
                },
                //France -> Nice
                new Accommodation
                {
                    AccommodationName="Riviera View Hotel",
                    AccommodationStars=4,
                    Destination=destinations[10],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=25
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Promenade des Anglais Lodges",
                    AccommodationStars=3,
                    Destination=destinations[10],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Azure Bay Resort",
                    AccommodationStars=5,
                    Destination=destinations[10],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                //France -> Avignon
                new Accommodation
                {
                    AccommodationName="Palais des Papes Hotel",
                    AccommodationStars=4,
                    Destination=destinations[11],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Pont Saint-Bénézet Inn",
                    AccommodationStars=3,
                    Destination=destinations[11],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Medieval Charm Lodges",
                    AccommodationStars=5,
                    Destination=destinations[11],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=50
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=25
                        },
                    }
                },
                //Turkey -> Istanbul
                new Accommodation
                {
                    AccommodationName="Bosphorus View Hotel",
                    AccommodationStars=5,
                    Destination=destinations[12],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=55
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Blue Mosque Grand Resort",
                    AccommodationStars=4,
                    Destination=destinations[12],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Grand Bazaar Homestay",
                    AccommodationStars=3,
                    Destination=destinations[12],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Turkey -> Cappadocia
                new Accommodation
                {
                    AccommodationName="Enchanted Chimneys Hotel",
                    AccommodationStars=4,
                    Destination=destinations[13],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Skyline Balloon Lodges",
                    AccommodationStars=3,
                    Destination=destinations[13],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Rocky Retreat Resort",
                    AccommodationStars=5,
                    Destination=destinations[13],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                //Turkey -> Antalya
                new Accommodation
                {
                    AccommodationName="Turquoise Coast Resort",
                    AccommodationStars=5,
                    Destination=destinations[14],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=55
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Ancient Ruins Hotel",
                    AccommodationStars=4,
                    Destination=destinations[14],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Düden Waterfalls Inn",
                    AccommodationStars=3,
                    Destination=destinations[14],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Brazil -> Rio de Janeiro
                new Accommodation
                {
                    AccommodationName="Copacabana Beachfront Resort",
                    AccommodationStars=5,
                    Destination=destinations[15],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Ipanema Views Hotel",
                    AccommodationStars=4,
                    Destination=destinations[15],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Christ the Redeemer Hotel",
                    AccommodationStars=3,
                    Destination=destinations[15],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Brazil -> Florianópolis
                new Accommodation
                {
                    AccommodationName="Magic Island Resort",
                    AccommodationStars=5,
                    Destination=destinations[16],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=55
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Sun-soaked Beachfront Hotel",
                    AccommodationStars=4,
                    Destination=destinations[16],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Cultural Oasis Lodges",
                    AccommodationStars=3,
                    Destination=destinations[16],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Brazil -> Salvador
                new Accommodation
                {
                    AccommodationName="Vibrant Afro-Brazilian Resort",
                    AccommodationStars=5,
                    Destination=destinations[17],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Historic Pelourinho Hotel",
                    AccommodationStars=4,
                    Destination=destinations[17],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Sunset Views Homestay",
                    AccommodationStars=3,
                    Destination=destinations[17],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Greece -> Athens
                new Accommodation
                {
                    AccommodationName="Acropolis View Hotel",
                    AccommodationStars=4,
                    Destination=destinations[18],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=6
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=40
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Parthenon Panorama Inn",
                    AccommodationStars=3,
                    Destination=destinations[18],
                    AccommodationType=accommodationType[1],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=4
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=25
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=10
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Ancient Agora Retreat",
                    AccommodationStars=5,
                    Destination=destinations[18],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=50
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=25
                        },
                    }
                },
                //Greece -> Santorini
                new Accommodation
                {
                    AccommodationName="Aegean Views Hotel",
                    AccommodationStars=5,
                    Destination=destinations[19],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Sunset Bliss Resort",
                    AccommodationStars=3,
                    Destination=destinations[19],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Caldera Charm Homestay",
                    AccommodationStars=3,
                    Destination=destinations[19],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Greece -> Crete
                new Accommodation
                {
                    AccommodationName="Minoan Majesty Hotel",
                    AccommodationStars=5,
                    Destination=destinations[20],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Mountain Retreat Resort",
                    AccommodationStars=4,
                    Destination=destinations[20],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Agora Atmosphere Homestay",
                    AccommodationStars=3,
                    Destination=destinations[20],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Mexico -> Mexico City
                new Accommodation
                {
                    AccommodationName="Zocalo Grand Hotel",
                    AccommodationStars=5,
                    Destination=destinations[21],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Teotihuacan Retreat Resort",
                    AccommodationStars=4,
                    Destination=destinations[21],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Colonial Charm Homestay",
                    AccommodationStars=3,
                    Destination=destinations[21],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Mexico -> Tulum
                new Accommodation
                {
                    AccommodationName="Caribbean Breeze Hotel",
                    AccommodationStars=5,
                    Destination=destinations[22],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Mayan Ruins Resort",
                    AccommodationStars=4,
                    Destination=destinations[22],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Turquoise Tranquility Homestay",
                    AccommodationStars=3,
                    Destination=destinations[22],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Mexico -> Cancun
                new Accommodation
                {
                    AccommodationName="Paradise Peninsula Hotel",
                    AccommodationStars=4,
                    Destination=destinations[23],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Vibrant Nightlife Resort",
                    AccommodationStars=4,
                    Destination=destinations[23],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Mayan Riviera Homestay",
                    AccommodationStars=3,
                    Destination=destinations[23],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Russia -> Saint Petersburg
                new Accommodation
                {
                    AccommodationName="Hermitage Heritage Hotel",
                    AccommodationStars=5,
                    Destination=destinations[24],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Winter Palace Resort",
                    AccommodationStars=4,
                    Destination=destinations[24],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Neva Views Homestay",
                    AccommodationStars=3,
                    Destination=destinations[24],
                    AccommodationType=accommodationType[2],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Russia -> Moscow
                new Accommodation
                {
                    AccommodationName="Red Square Luxury Hotel",
                    AccommodationStars=5,
                    Destination=destinations[25],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=12
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=15
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=60
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=30
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Kremlin Grand Resort",
                    AccommodationStars=4,
                    Destination=destinations[25],
                    AccommodationType=accommodationType[0],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=10
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=45
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=20
                        },
                    }
                },
                new Accommodation
                {
                    AccommodationName="Moscow Lights Inn",
                    AccommodationStars=3,
                    Destination=destinations[25],
                    AccommodationType=accommodationType[3],
                    Rooms=new List<Room>
                    {
                        new Room
                        {
                            RoomType=roomTypes[0],
                            NumberOfRooms=5
                        },
                        new Room
                        {
                            RoomType=roomTypes[1],
                            NumberOfRooms=8
                        },
                        new Room
                        {
                            RoomType=roomTypes[2],
                            NumberOfRooms=30
                        },
                        new Room
                        {
                            RoomType=roomTypes[3],
                            NumberOfRooms=15
                        },
                    }
                },
                //Russia -> Lake Baikal
                new Accommodation
                {
                    AccommodationName="Breathtaking Baikal Hotel",
                    AccommodationStars=4,
                    Destination=destinations[26],
                    AccommodationType=accommodationType[0]
                },
                new Accommodation
                {
                    AccommodationName="Wilderness Retreat Lodges",
                    AccommodationStars=3,
                    Destination=destinations[26],
                    AccommodationType=accommodationType[3]
                },
                new Accommodation
                {
                    AccommodationName="Siberian Serenity Resort",
                    AccommodationStars=5,
                    Destination=destinations[26],
                    AccommodationType=accommodationType[0]
                },
            };

            var admin = (Admin)WithUseCases(new Admin
            {
                FirstName = "Agency",
                LastName = "Admin",
                Email = "admin@flextravel.local",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Phone = "+381601234567",
                Address = "Knez Mihailova 1",
                City = "Belgrade"
            });

            var customer = (RegularUser)WithUseCases(new RegularUser
            {
                FirstName = "Petar",
                LastName = "Petrovic",
                Email = "korisnik@flextravel.local",
                Password = BCrypt.Net.BCrypt.HashPassword("Korisnik123!"),
                Phone = "+381607654321",
                Address = "Bulevar oslobodjenja 10",
                City = "Novi Sad"
            });

            // Agencies, trips, prices and reservations: sample data, see BookingSampleData.
            var booking = BookingSampleData.Create(
                destinations,
                accommodations,
                customer,
                admin,
                password => BCrypt.Net.BCrypt.HashPassword(password),
                WithUseCases);

            // Two saves (see the notifications below), so one transaction keeps the seed all-or-nothing.
            using var transaction = _context.Database.BeginTransaction();

            _context.Continent.AddRange(continents);
            _context.Country.AddRange(country);
            _context.Destination.AddRange(destinations);
            _context.Climate.AddRange(climates);
            _context.AccommodationsType.AddRange(accommodationType);
            _context.RoomType.AddRange(roomTypes);
            _context.Accommodation.AddRange(accommodations);
            _context.User.AddRange(admin, customer);
            _context.Transport.AddRange(booking.Transports);
            _context.Agency.AddRange(booking.Agencies);
            _context.User.AddRange(booking.Users);
            _context.Trip.AddRange(booking.Trips);
            _context.Reservation.AddRange(booking.Reservations);
            _context.AttractionCategory.AddRange(booking.Attractions.Categories);
            _context.Attraction.AddRange(booking.Attractions.Attractions);
            _context.PromoCode.AddRange(booking.PromoCodes);
            _context.Review.AddRange(booking.Reviews);
            _context.WaitlistEntry.AddRange(booking.Waitlist);
            _context.UserFavorite.AddRange(booking.Favorites);
            _context.PriceAlert.AddRange(booking.PriceAlerts);

            _context.SaveChanges();

            // Notifications link to reservations, so they need the IDs from the first save.
            _context.Notification.AddRange(BookingSampleData.Notifications(booking));
            _context.SaveChanges();
            transaction.Commit();
        }

        // Test accounts are for local development only (the seed is only reachable in Development).
        private static User WithUseCases(User user)
        {
            var useCaseIds = user switch
            {
                Admin => UserUseCaseConstants.AdminUseCaseIds,
                AgencyEmployee => UserUseCaseConstants.AgencyEmployeeUseCaseIds,
                _ => UserUseCaseConstants.RegularUserUseCaseIds
            };

            foreach (var useCaseId in useCaseIds)
            {
                user.UseCases.Add(new UserUseCase { UseCaseId = useCaseId });
            }

            return user;
        }
    }
}

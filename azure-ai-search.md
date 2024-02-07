# Azure AI Search
**99.9% uptime SLA**  
Service built on top of **Apache Lucene**  

Azure AI Search provides the infrastructure and tools to create search solutions that extract data from various structured, semi-structured, and non-structured documents.
![image](https://github.com/Glareone/OpenAI-and-ChatGPT-meet-.Net/assets/4239376/77d52141-5f4e-4996-9def-1e14051fa344)

## Features
* Data from any source: accepts data from any source provided in JSON format, with auto crawling support for selected data sources in Azure.
* Full text search and analysis: offers full text search capabilities supporting both simple query and full Lucene query syntax.
* AI powered search: has Azure AI capabilities built in for image and text analysis from raw content.
* Multi-lingual offers linguistic analysis for 56 languages to intelligently handle phonetic matching or language-specific linguistics. Natural language processors available in Azure AI Search are also used by Bing and Office.
* Geo-enabled: supports geo-search filtering based on proximity to a physical location.
* Configurable user experience: has several features to improve the user experience including autocomplete, autosuggest, pagination, and hit highlighting.

## Data Ingestion
Original Data Artifacts may come from: 
1. Azure Blob Storage
2. Azure SQL Database
3. Azure Cosmos DB.
4. Regardless of where your data originates, if you can provide it as a JSON document, the search engine can index it.
![image](https://github.com/Glareone/OpenAI-and-ChatGPT-meet-.Net/assets/4239376/2cc6eace-3923-4907-83ff-6d82a3422fe8)

## Indexers
Most indexers support change detection, which makes data refresh a simpler exercise.

## Indexers. Data Enrichment
Indexers also support AI enrichment.  
You can attach a skillset that applies a sequence of AI skills to enrich the data, making it more searchable.  
Optionally, enriched content can be sent to a knowledge store, which stores output from an AI enrichment pipeline in tables and blobs in Azure Storage for independent analysis or downstream processing.  

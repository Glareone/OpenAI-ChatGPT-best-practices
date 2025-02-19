# Extract Key Phrases
Might be useful if you want to improve search in AI search with Hybrid or Semantic Query Type.  

1. Extact Key Phrases from the text:

> https://ai-language-key-phrases-extraction.cognitiveservices.azure.com/language/:analyze-text?api-version=2022-05-01
> H- Content-Type application/json
> H- Ocp-Apim-Subscription-Key: "Key"
> body:
```
{
    "kind": "KeyPhraseExtraction",
    "parameters": {
        "modelVersion": "latest"
    },
    "analysisInput":{
        "documents":[
            {
                "id":"1",
                "language":"en",
                "text": "total cash on Statement of cash position August 31, 2019 for 212 N 1 st Lofts"
            }
        ]
    }
}
```

<img width="955" alt="image" src="https://github.com/user-attachments/assets/ea708dd2-de00-4d81-8ab9-ff8aba10013a" />

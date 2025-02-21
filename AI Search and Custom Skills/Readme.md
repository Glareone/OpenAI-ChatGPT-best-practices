# AI Search and Skills
## Default Skills and their usage

## Custom Skills using Azure Function
As starting point i used [lab from learn portal](https://learn.microsoft.com/en-us/training/modules/create-azure-ai-custom-skill/3-add-custom-skill)  
  
![image](https://github.com/user-attachments/assets/0bb6bdda-1db4-409d-bf43-195b54d3c7c9)

```json
{
    "skills": [
      ...,
      {
        "@odata.type": "#Microsoft.Skills.Custom.WebApiSkill",
        "description": "<custom skill description>",
        "uri": "https://<web_api_endpoint>?<params>",
        "httpHeaders": {
            "<header_name>": "<header_value>"
        },
        "context": "/document/<where_to_apply_skill>",
        "inputs": [
          {
            "name": "<input1_name>",
            "source": "/document/<path_to_input_field>"
          }
        ],
        "outputs": [
          {
            "name": "<output1_name>",
            "targetName": "<optional_field_name>"
          }
        ]
      }
  ]
}

```

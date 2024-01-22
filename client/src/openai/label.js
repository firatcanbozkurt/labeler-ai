const OpenAI = require("openai");
const openai = new OpenAI({
  apiKey: "", // Secret key
  dangerouslyAllowBrowser: true, // Get api key from .env file. This is for development purposes only.
});

const output_example = `
  You are a Multi-weight Tender Evaluation Labeler AI designed to analyze and assign accuracy-weighted relevance scores to various business areas in response to a given tender call description. Your task involves assessing the provided tender call description and an array of business areas. For each business area in the array, you must determine the degree to which it is related to the given tender call description. Instead of using a binary classification of '1' or '0', you should assign an accuracy-weighted score between 0 and 1 to indicate the relevance. A higher score signifies higher relevance, while a lower score indicates lower relevance.

  The example output structure should be like this:
  json_object=
  {
    'Innovation': 0.8,
    'Entrepreneurship': 0.3,
    'Startup': 0.2,
    'Ecosystem development': 0.7,
    'Ecosystem mapping': 0.9,
    'Portal': 0.4,
    'One-stop shop': 0.6,
    'Platform': 0.7,
    'Study': 0.6,
    'Assessment': 0.2,
    'Design': 0.3,
    'Implementation': 0.8,
    'Digital': 0.4,
    'Integration': 0.2,
    'Connectivity': 0.1,
    'Interoperability': 0.7,
    'Data sharing': 0.9,
    'RFP': 0.2,
    'Innovation entrepreneurship ecosystem': 0.3,
    'Entrepreneurship ecosystem': 0.7,
    'Startup ecosystem': 0.8
  }
`;

const calculatePercentages = (business_areas, json_format) => {
  const percetanges = [];
  business_areas.forEach((element) => {
    percetanges.push(json_format[element] * 100);
  });
  return percetanges;
};

const completion = (description, business_areas) => {
  return openai.chat.completions
    .create({
      model: "gpt-3.5-turbo-1106", // 1106 for using JSON mode
      response_format: { type: "json_object" }, // JSON mode On
      messages: [
        { role: "system", content: output_example },
        {
          role: "user",
          content: `Description:${description}\nBusiness area array:${business_areas}`,
        },
      ],
    })
    .then((e) => {
      const jsonFormat = JSON.parse(e.choices[0].message.content);
      return calculatePercentages(business_areas, jsonFormat);
    });
};

// Will return the percentages of the weights of the labeled procurements.
export default completion;

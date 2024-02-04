# NLP. Natural Language Processing
Tokenization is the first step in Natural Language Processing is to split your corpus (Text Body) onto Tokens. In simplest scenario each token equals to one word, but in reality it's not that easy:  
"we choose to go to the moon":
1. we
2. choose
3. to
4. go
5. the
6. moon

## Language Processing Concepts and Strategies
* Text normalization: Before generating tokens, you may choose to normalize the text by removing punctuation and changing all words to lower case. For analysis that relies purely on word frequency, this approach improves overall performance. However, some semantic meaning may be lost - for example, consider the sentence "Mr Banks has worked in many banks.". You may want your analysis to differentiate between the person Mr Banks and the banks in which he has worked. You may also want to consider "banks." as a separate token to "banks" because the inclusion of a period provides the information that the word comes at the end of a sentence
* Stop word removal. Stop words are words that should be excluded from the analysis. For example, "the", "a", or "it" make text easier for people to read but add little semantic meaning. By excluding these words, a text analysis solution may be better able to identify the important words.
* n-grams are multi-term phrases such as "I have" or "he walked". A single word phrase is a unigram, a two-word phrase is a bi-gram, a three-word phrase is a tri-gram, and so on. By considering words as groups, a machine learning model can make better sense of the text.
* Stemming is a technique in which algorithms are applied to consolidate words before counting them, so that words with the same root, like "power", "powered", and "powerful", are interpreted as being the same token.

## TF-IDF. Term Frequency - inverse document frequency  
It's a common technique in which a score is calculated based on how often a word or term appears in one document compared to its more general frequency across the entire collection of documents.

## ML Logistic Regression for text classification. 
Another useful text analysis technique is to use a classification algorithm, such as logistic regression, to train a machine learning model that classifies text based on a known set of categorizations
* Can be useful in sentiment analysis and\or opinion mining.

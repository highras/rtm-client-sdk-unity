# RTM Client Unity SDK Value-Added API Docs

# Index

[TOC]

### Set Translated Language

	//-- Async Method
	public bool SetTranslatedLanguage(DoneDelegate callback, TranslateLanguage targetLanguage, int timeout = 0);
	
	//-- Sync Method
	public int SetTranslatedLanguage(TranslateLanguage targetLanguage, int timeout = 0);

Set target language to enable auto-translating.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `TranslateLanguage targetLanguage`

	Target language enum.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async calling is start.
	* false: Start async calling is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means calling successed.

	Others are the reason for calling failed.

### Translate

	//-- Async Method
	public bool Translate(Action<TranslatedMessage, int> callback, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0);
	
	//-- Sync Method
	public int Translate(out TranslatedMessage translatedMessage, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0);

Translate text to target language.

Parameters:

+ `Action<TranslatedMessage>, int> callback`

	Callabck for async method.  
	First `TranslatedMessage` is translation message result, please refer [TranslatedMessage](Structures.md#TranslatedMessage);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out TranslatedMessage translatedMessag`

	The translation message result, please refer [TranslatedMessage](Structures.md#TranslatedMessage).

+ `string text`

	The text need to be translated.

+ `TranslateLanguage destinationLanguage`

	Target language enum.

+ `TranslateLanguage sourceLanguage`

	Source language enum. Value `TranslateLanguage.None` means automatic recognition.

+ `TranslateType type`

	TranslateType.Chat or TranslateType.Mail. Default is TranslateType.Chat.

+ `ProfanityType profanity`

	Profanity filter action.

	* ProfanityType.Off (**Default**)
	* ProfanityType.Stop
	* ProfanityType.Censor

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async calling is start.
	* false: Start async calling is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means calling successed.

	Others are the reason for calling failed.

### Profanity

	//-- Async Method
	public bool Profanity(Action<string, List<string>, int> callback, string text, bool classify = false, int timeout = 0);
	
	//-- Sync Method
	public int Profanity(out string resultText, out List<string> classification, string text, bool classify = false, int timeout = 0);

Sensitive words detected and filter.

Parameters:

+ `Action<string, List<string>, int> callback`

	Callabck for async method.  
	First `string` is the processed text by sensitive words detecting and filtering;  
	Second `List<string>` is the classifications of the sensitive words for the original text.  
	If `classify` is `false`, the second parameter will be null.  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out string resultText`

	Processed text by sensitive words detecting and filtering.

+ `out List<string> classification`

	 Classifications of the sensitive words for the original text.  
	 If `classify` is `false`, this parameter will be null.

+ `string text`

	The text need to be detected and filtered.

+ `bool classify`

	Whether to classify the sensitive words.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async calling is start.
	* false: Start async calling is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means calling successed.

	Others are the reason for calling failed.


### Transcribe

	//-- Async Method
	public bool Transcribe(Action<string, string, int> callback, byte[] audio, int timeout = 120);
	public bool Transcribe(Action<string, string, int> callback, byte[] audio, bool filterProfanity, int timeout = 120);
	
	//-- Sync Method
	public int Transcribe(out string resultText, out string resultLanguage, byte[] audio, int timeout = 120);
	public int Transcribe(out string resultText, out string resultLanguage, byte[] audio, bool filterProfanity, int timeout = 120);

Speech Recognition.

Parameters:

+ `Action<string, string, int> callback`

	Callabck for async method.  
	First `string` is the text converted from recognized speech;  
	Second `string` is the recognized language.  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out string resultText`

	The text converted from recognized speech.

+ `out string resultLanguage`

	The recognized language.

+ `byte[] audio`

	Speech data.

+ `filterProfanity`

	Enable or disable sensitive words detected and filter.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async calling is start.
	* false: Start async calling is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means calling successed.

	Others are the reason for calling failed.


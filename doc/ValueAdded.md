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
    public bool Translate(Action<TranslatedInfo, int> callback, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0);
	
	//-- Sync Method
    public int Translate(out TranslatedInfo translatedinfo, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0);

Translate text to target language.

Parameters:

+ `Action<TranslatedInfo>, int> callback`

	Callabck for async method.  
	First `TranslatedInfo` is translation message result, please refer [TranslatedInfo](Structures.md#TranslatedInfo);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out TranslatedInfo translatedinfo`

	The translation message result, please refer [TranslatedInfo](Structures.md#TranslatedInfo).

+ `string text`

	The text need to be translated.

+ `string destinationLanguage`

	Target language. Please refer the 'Language support' section in [document](https://docs.ilivedata.com/stt/production/) for language value.

+ `string sourceLanguage`

	Source language. Empty string means automatic recognition. Please refer the 'Language support' section in [document](https://docs.ilivedata.com/stt/production/) for language value.

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

### Profanity (Deprecated)

	//-- Async Method
	public bool Profanity(Action<string, List<string>, int> callback, string text, bool classify = false, int timeout = 0);
	
	//-- Sync Method
	public int Profanity(out string resultText, out List<string> classification, string text, bool classify = false, int timeout = 0);

**`Profanity` is deprecated, please use [TextCheck](#TextCheck)  instead.**

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


### SpeechToText

	//-- Async Method
	public bool SpeechToText(Action<string, string, int> callback, byte[] audioBinaryContent, string language, string codec = null, int sampleRate = 0, int timeout = 120);
	
	//-- Sync Method
	public int SpeechToText(out string resultText, out string resultLanguage, byte[] audioBinaryContent, string language, string codec = null, int sampleRate = 0, int timeout = 120);

Speech Recognition, convert speech to text.

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

+ `byte[] audioBinaryContent`

	Speech binary data.

+ `language`

	Speech language when recording. Available language please refer the documents in [https://www.ilivedata.com/](https://docs.ilivedata.com/stt/production/).

	[Current Chinese document](https://docs.ilivedata.com/stt/production/)

+ `codec`

	Codec for speech binary. If codec is `null` means `AMR_WB`.

+ `sampleRate`

	Sample rate for speech binary. If `0` means 16000.

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


### TextCheck

	//-- Async Method
	public bool TextCheck(Action<TextCheckResult, int> callback, string text, int timeout = 120);
	
	//-- Sync Method
	public int TextCheck(out TextCheckResult result, string text, int timeout = 120);

Text moderation.

Parameters:

+ `Action<TextCheckResult, int> callback`

	Callabck for async method.  
	First `TextCheckResult` is the result for text moderation;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.  
	`TextCheckResult` can be refered [TextCheckResult](Structures.md#TextCheckResult).

+ `out TextCheckResult result`

	The result for text moderation. Please refer [TextCheckResult](Structures.md#TextCheckResult).

+ `string text`

	The text need to be audited.

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


### ImageCheck

	//-- Async Method
	public bool ImageCheck(Action<CheckResult, int> callback, string imageUrl, int timeout = 120);
	public bool ImageCheck(Action<CheckResult, int> callback, byte[] imageContent, int timeout = 120);
	
	//-- Sync Method
	public int ImageCheck(out CheckResult result, string imageUrl, int timeout = 120);
	public int ImageCheck(out CheckResult result, byte[] imageContent, int timeout = 120);

Image review.

Parameters:

+ `Action<CheckResult, int> callback`

	Callabck for async method.  
	First `CheckResult` is the result for image review;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.  
	`CheckResult` can be refered [CheckResult](Structures.md#CheckResult).

+ `out CheckResult result`

	The result for image review. Please refer [CheckResult](Structures.md#CheckResult).

+ `string imageUrl`

	Image's http/https url for auditing.

+ `byte[] imageContent`

	Image binary data for auditing.

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


### AudioCheck

	//-- Async Method
	public bool AudioCheck(Action<CheckResult, int> callback, string audioUrl, string language, string codec = null, int sampleRate = 0, int timeout = 120);
	public bool AudioCheck(Action<CheckResult, int> callback, byte[] audioContent, string language, string codec = null, int sampleRate = 0, int timeout = 120);
	
	//-- Sync Method
	public int AudioCheck(out CheckResult result, string audioUrl, string language, string codec = null, int sampleRate = 0, int timeout = 120);
	public int AudioCheck(out CheckResult result, byte[] audioContent, string language, string codec = null, int sampleRate = 0, int timeout = 120);

Audio check.

Parameters:

+ `Action<CheckResult, int> callback`

	Callabck for async method.  
	First `CheckResult` is the result for audio checking;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.  
	`CheckResult` can be refered [CheckResult](Structures.md#CheckResult).

+ `out CheckResult result`

	The result for audio checking. Please refer [CheckResult](Structures.md#CheckResult).

+ `string audioUrl`

	Http/https url for speech binary to be checking.

+ `byte[] audioContent`

	Audio binary data for checking.

+ `language`

	Audio language when recording. Available language please refer the documents in [https://www.ilivedata.com/](https://docs.ilivedata.com/stt/production/).

	[Current Chinese document](https://docs.ilivedata.com/audiocheck/techdoc/submit/)  
	[Current Chinese document (live audio)](https://docs.ilivedata.com/audiocheck/livetechdoc/livesubmit/)

+ `codec`

	Codec for audio content. If codec is `null` means `AMR_WB`.

+ `sampleRate`

	Sample rate for audio content. If `0` means 16000.

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


### VideoCheck

	//-- Async Method
	public bool VideoCheck(Action<CheckResult, int> callback, string videoUrl, string videoName, int timeout = 120);
	public bool VideoCheck(Action<CheckResult, int> callback, byte[] videoContent, string videoName, int timeout = 120);
	
	//-- Sync Method
	public int VideoCheck(out CheckResult result, string videoUrl, string videoName, int timeout = 120);
	public int VideoCheck(out CheckResult result, byte[] videoContent, string videoName, int timeout = 120);

Video review.

Parameters:

+ `Action<CheckResult, int> callback`

	Callabck for async method.  
	First `CheckResult` is the result for video review;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.  
	`CheckResult` can be refered [CheckResult](Structures.md#CheckResult).

+ `out CheckResult result`

	The result for video review. Please refer [CheckResult](Structures.md#CheckResult).

+ `string videoUrl`

	Video's http/https url for auditing.

+ `byte[] videoContent`

	Video binary data for auditing.

+ `string videoName`

	Video name.

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


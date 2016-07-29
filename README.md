# Introducing See4Me

*See4Me* is a cross-platform app that shows how [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services) work. Just start the app, point the smartphone to a subject and tap: in a few instants, the app will describe and speech what it sees. Moreover, if it recognizes one or more people, it tries to detect their ages and emotions. The app is also able to automatically translate descriptions using [Microsoft Bing Translator](https://www.microsoft.com/en-us/translator/products.aspx).

**Getting started**

First, you must obtain a Cognitive API subscription keys by following instructions in [Microsoft Cognitive Services subscription](https://www.microsoft.com/cognitive-services/en-us/sign-up). Then, you also need to subscribe to [Microsoft Translator Service](https://datamarket.azure.com/dataset/bing/microsofttranslator) and register a [new application](https://datamarket.azure.com/developer/applications).

You need to insert these service keys (Subscription Key for Vision and Emotion, ClientId and ClientSecret for Translator Service) in the *Src/See4Me.Shared/ServiceKeys.cs* file.

**Screenshots**

*Windows 10 version*

<img src="https://raw.githubusercontent.com/DotNetToscana/See4Me/master/Screenshots/Windows/DescribeImage01.png" width="400" height="225">
<img src="https://raw.githubusercontent.com/DotNetToscana/See4Me/master/Screenshots/Windows/DescribeImage02.png" width="400" height="225">

**Contribute**

The project is continually evolving. If you want to contribute, feel free make to pull requests or get in touch with us! 

**Code of Conduct**

See4Me uses the Microsoft Cognitive Services, see https://www.microsoft.com/cognitive-services/. Developers using this project are expected to follow the “Developer Code of Conduct for Microsoft Cognitive Services” at http://go.microsoft.com/fwlink/?LinkId=698895. 

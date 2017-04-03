# How to Register Cognitive Services

In order to use Microsoft Cognitive Services, you need to get the corresponding service keys by following the instructions available in the **Microsoft Cognitive Services subscription** page:

1. Go to https://www.microsoft.com/cognitive-services/en-us/sign-up and login with your Microsoft, GitHub or LinkedIn account:

<img src="https://raw.githubusercontent.com/DotNetToscana/See4Me/master/Docs/Images/CognitiveServices_Signup.png">

2. After logging in, click the **Request new trials** button and select the products you want to subscribe. For our purposes, we need the following ones:
    1. Computer Vision
    2. Face

In the description column you can see the free quota limit for each service. Check the option *I agree to the Microsoft Cognitive Services Terms and Microsoft Privacy Statement* and click **Subscribe**.

3. You'll be redirect to the **My free subscription** pages, that contains all the products for which you have a subscription. For each one there is a couple of Keys, for example:

<img src="https://raw.githubusercontent.com/DotNetToscana/See4Me/master/Docs/Images/CognitiveServices_Keys.png">

By clicking on **Show**, you'll see the keys you'll need to enter in the app setting page to use Cognitive Services features.


Then, if you want to use translation feature, you also need an Azure subscription to activate **Microsoft Translator Service**. Go to https://portal.azure.com/#create/Microsoft.CognitiveServices/apitype/TextTranslation to start the service creation wizard:

<img src="https://raw.githubusercontent.com/DotNetToscana/See4Me/master/Docs/Images/TranslatorService_Creation.png">

Enter an account name and be sure that, under API type, the value *Translator Text API* is selected. Then insert all the other information. In particular, in the *Pricing tier* box you can select how to pay the service: as all the resources available on Azure, you pay what you use. There is a free tier that allows to translate up to 2M characters a month. When you have set all the properties, confirm by clicking on **Create**. After the service deployment is completed, go to its **Keys** section:

<img src="https://raw.githubusercontent.com/DotNetToscana/See4Me/master/Docs/Images/TranslatorService_Keys.png">

You need to enter either Key 1 or Key 2 in the app setting page to activate translator feature. Note that it may take up to 10 minutes for the newly (re)generated keys to take effect.

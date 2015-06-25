# SharePoint 2013 Claims Authentication Debugger

Small tool that helps you troubleshooting Claims Authentication in SharePoint 2013 (including Session Timeout and Sliding Expiration).

Just deploy the Feature to the Web Application you want to test. Then on the same server you can launch the application which will show the logs in real time:

![](https://cdn.auth0.com/docs/img/sharepoint-fedauth.png)

Note that this also implements Sliding Expiration in the HttpModule as described here: https://msdn.microsoft.com/en-us/library/hh446526.aspx

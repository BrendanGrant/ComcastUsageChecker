# ComcastUsageChecker

With data usage caps comes the need to monitor ones usage through the month, and logging into the website manually gets old, so like many I wrote some code to automate the process.

One warning about this code, it is ugly, and I am not proud of it.

In this code there are two methods, one which *should* work and uses automatic redirects, and one that redirects manually.

The first *should* work, however the location sent from the server to a oauth/authorize path is missing some arguments needed to proceed, while the second method inserts the needed data in a dirty way.

Again, I'm not pleased with this code, but figured it was useful enough that someone else may have a use.

General flow was loosly based off of https://github.com/lachesis/comcast/tree/bd18d804b7271b925fa1ca67a8eb909ec6554a03 after much head banging over redirects not working as expected.

Requirements:
* .NET Core 2.0

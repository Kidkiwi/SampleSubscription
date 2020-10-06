public static class ExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static string FixIAPTitle(this string str)
    {
        if (str.Contains("("))
            str = str.Substring(0, str.IndexOf("("));

        return str;
    }
}
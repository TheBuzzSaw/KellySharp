namespace KellySharp.Sdl
{
    public static class SdlExtensions
    {
        public static string ResolveName(string methodName)
        {
            if (methodName.StartsWith("Gl"))
                return "SDL_GL_" + methodName.Substring(2);
            
            return "SDL_" + methodName;
        }
    }
}
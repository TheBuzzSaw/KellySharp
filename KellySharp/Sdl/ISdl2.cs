using System;

namespace KellySharp.Sdl
{
    public interface ISdl2
    {
        int Init(uint flags);
        void Quit();
        string GetError();

        IntPtr CreateRGBSurface(
            uint flags,
            int width,
            int height,
            int depth,
            uint rmask,
            uint gmask,
            uint bmask,
            uint amask);

        IntPtr CreateRGBSurfaceFrom(
            IntPtr pixels,
            int width,
            int height,
            int depth,
            int pitch,
            uint rmask,
            uint gmask,
            uint bmask,
            uint amask);
        IntPtr CreateWindow(string title, int x, int y, int w, int h, uint flags);
        void DestroyWindow(IntPtr window);
        void FreeSurface(IntPtr surface);
        int GetDisplayBounds(int displayIndex, out SdlRect rect);
        int GetDisplayUsableBounds(int displayIndex, out SdlRect rect);
        uint GetWindowFlags(IntPtr window);
        IntPtr GetWindowFromID(uint windowId);
        uint GetWindowID(IntPtr window);
        void GetWindowSize(IntPtr window, out int width, out int height);
        int GetWindowDisplayIndex(IntPtr window);
        IntPtr GetWindowSurface(IntPtr window);
        IntPtr CreateSoftwareRenderer(IntPtr surface);
        void MaximizeWindow(IntPtr window);
        int SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a);
        int RenderClear(IntPtr renderer);
        void RenderPresent(IntPtr renderer);
        void RestoreWindow(IntPtr window);
        void SetWindowBordered(IntPtr window, int bordered);
        int SetWindowFullscreen(IntPtr window, uint flags);
        void SetWindowPosition(IntPtr window, int x, int y);
        void SetWindowResizable(IntPtr window, int resizable);
        void SetWindowSize(IntPtr window, int width, int height);
        int UpdateWindowSurface(IntPtr window);
        ulong GetPerformanceFrequency();
        ulong GetPerformanceCounter();
        int PollEvent(byte[] eventData);
        int UpperBlit(
            IntPtr source,
            int[]? sourceRectangle,
            IntPtr destination,
            int[]? destinationRectangle);
        int WaitEvent(byte[] eventData);
        uint RegisterEvents(int numEvents);
        int PushEvent(byte[] eventData);

        IntPtr GlCreateContext(IntPtr window);
        void GlDeleteContext(IntPtr context);
        int GlMakeCurrent(IntPtr window, IntPtr context);
        int GlSetSwapInterval(int interval);
        int GlSetAttribute(int attribute, int value);
        void GlSwapWindow(IntPtr window);
        
        int VulkanCreateSurface(IntPtr window, IntPtr instance, IntPtr surface);
        void VulkanGetDrawableSize(IntPtr window, out int w, out int h);
        int VulkanGetInstanceExtensions(IntPtr window, out uint pCount, IntPtr pNames);
        int VulkanLoadLibrary(string path);
        void VulkanUnloadLibrary();
    }
}
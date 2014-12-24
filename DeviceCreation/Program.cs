/*
* Copyright (c) 2007-2009 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using System.Drawing;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Windows;

namespace MiniTri
{
    static class Program
    {
     
        [STAThread]
        static void Main()
        {
            var form = new DeviceCreation.Form1();
            var present = new PresentParameters()
            {
                BackBufferWidth = form.ClientSize.Width,
                BackBufferHeight = form.ClientSize.Height
            };
            var device = new Device(new Direct3D(), 0, DeviceType.Hardware, form.getHandle(), CreateFlags.HardwareVertexProcessing, present);

            var texture = Texture.FromFile(device, "mypng.png");
            SurfaceDescription description = texture.GetLevelDescription(0);
            var center = new SlimDX.Vector3(description.Width, description.Height, 0) * 0.5f;
            var scale = 0.2f;
            var sprite = new Sprite(device);
            
            form.Resize += (o, e) => {
                //important without this device.Reset() will crash
                sprite.Dispose();

                // resize backbuffer to new form dimensions
                present.BackBufferWidth = form.ClientSize.Width;
                present.BackBufferHeight = form.ClientSize.Height;
                
                device.Reset(present);
                
                //recreate sprite object
                sprite = new Sprite(device);
            };

            MessagePump.Run(form, () =>
            {
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();

                sprite.Transform = SlimDX.Matrix.Scaling(scale, scale, 0);
                SlimDX.Color4 color = new SlimDX.Color4(Color.White);
                sprite.Begin(SpriteFlags.AlphaBlend);
                SlimDX.Vector3 position = new SlimDX.Vector3(0, 0, 0);
                
                sprite.Draw(texture, position, position, color);
                
                sprite.End();

                device.EndScene();
                device.Present();
            });

            foreach (var item in ObjectTable.Objects)
                item.Dispose();
        }
                
    }
}
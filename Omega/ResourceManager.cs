using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class ResourceManager
    {

        private static ResourceManager Instance;

        private Dictionary<string, Font> fonts;
        private Dictionary<string, Brush> brushes;
        private Dictionary<string, Pen> penes;
        private Dictionary<string, Bitmap> textures;


        private ResourceManager() {
            fonts = new Dictionary<string, Font>();
            brushes = new Dictionary<string, Brush>();
            penes = new Dictionary<string, Pen>();
            textures = new Dictionary<string, Bitmap>();
        }

        public void AddFont(string name, Font font)
        {
            fonts[name] = font;
        }
        public Font GetFont(string name)
        {
            return fonts[name];
        }

        public void AddPen(string name,Pen p)
        {
            penes[name] = p;
        }

        public Pen GetPen(string name)
        {
            return penes[name];
        }

        public void AddBrush(string name, Brush b)
        {
            brushes[name] = b;
        }
        public Brush GetBrush(string name)
        {
            return brushes[name];
        }

        public Bitmap LoadTexture(string path){

            if (!textures.ContainsKey(path))
            {
                Bitmap bmp = Image.FromFile(path, true) as Bitmap;

                textures.Add(path, bmp);
                return bmp;
            }
            else
                return textures[path];
        }
        public Bitmap GetTexture(string path)
        {
            return textures[path];
        }

        public static ResourceManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new ResourceManager();
            }
            return Instance;
        }


    }
}

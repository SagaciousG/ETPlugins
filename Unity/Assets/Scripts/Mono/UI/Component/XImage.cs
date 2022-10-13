using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class XImage : Image
    {
        [Flags]
        public enum FlipDirection
        {
            Horizontal    = 1 << 0,
            Vertical      = 2 << 1
        }
        
        [SerializeField]
        private string _spriteAsset;
        
        private Color _defaultColor;
        private Task<Sprite> _loadTask;
        private bool _gray;
        [SerializeField]
        private FlipDirection _flip;

        private Material _grayMat;
        private static readonly int Property = Shader.PropertyToID("show gray");


        protected override void Awake()
        {
            _defaultColor = color;
        }

        protected override void Start()
        {
            if (!string.IsNullOrEmpty(_spriteAsset))
            {
                sprite = SpritePool.Instance.Fetch(_spriteAsset);
                if (sprite == null)
                {
                    base.color = Color.clear;
                    base.sprite = null;
                }
                else
                {
                    base.color = _defaultColor;
                    base.sprite = sprite;
                }
            }
        }
        
        public bool gray
        {
            set
            {
                _gray = value;
                _grayMat ??= new Material(Shader.Find("UI/UIGray"));
                material = _grayMat;
                _grayMat.SetFloat(Property, value ? 0 : 1);
            }
            get => _gray;
        }
        

        
        public bool show
        {
            set
            {
                var c = color;
                c.a = value ? _defaultColor.a : 0;
                base.color = c;
            }
            get => color.a > 0;
        }

        public FlipDirection Flip
        {
            set
            {
                _flip = value;
                UpdateGeometry();
            }
            get => _flip;
        }


        
        public override Color color
        {
            get => base.color;
            set
            {
                base.color = value;
                _defaultColor = value;
            }
        }

        public string Skin
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _spriteAsset = value;
                    base.color = Color.clear;
                    return;
                }
                if(value == _spriteAsset)
                    return;
                
                if (sprite != null && Regex.IsMatch(value, $"{sprite.name}(.png|.jpg)"))
                {
                    base.color = _defaultColor;
                    return;
                }
                if (!string.IsNullOrEmpty(_spriteAsset))
                {
                    SpritePool.Instance.Collect(_spriteAsset, base.sprite);
                    sprite = null;
                }
                _spriteAsset = value;
                sprite = SpritePool.Instance.Fetch(value);
                if (sprite == null)
                {
                    base.color = Color.clear;
                    base.sprite = null;
                }
                else
                {
                    base.color = _defaultColor;
                    base.sprite = sprite;
                }
            }
            get => _spriteAsset;
        }

        public void UpdateGraph()
        {
            UpdateGeometry();
        }
        
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
 
            Vector2 rectCenter = rectTransform.rect.center;

            int vertCount = toFill.currentVertCount;
            for (int i = 0; i < vertCount; i++)
            {
                UIVertex uiVertex = new UIVertex();
                toFill.PopulateUIVertex(ref uiVertex, i);

                Vector3 pos = uiVertex.position;
                var x = (_flip & FlipDirection.Horizontal) == FlipDirection.Horizontal ? (pos.x + (rectCenter.x - pos.x) * 2) : pos.x;
                var y = (_flip & FlipDirection.Vertical) == FlipDirection.Vertical ? (pos.y + (rectCenter.y - pos.y) * 2) : pos.y;
                uiVertex.position = new Vector3(x, y, pos.z);

                toFill.SetUIVertex(uiVertex, i);
            }
        }

        protected override void OnDestroy()
        {
            // if (!string.IsNullOrEmpty(_spriteAsset) && sprite != null)
            // {
            //     SpritePool.Instance?.Collect(_spriteAsset, base.sprite);
            // }
            base.OnDestroy();
        }
    }
}
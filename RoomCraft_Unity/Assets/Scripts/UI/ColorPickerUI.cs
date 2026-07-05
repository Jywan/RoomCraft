using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// Hue + Saturation를 원형 휠로, Value는 별도 슬라이더로 선택하는 색상 피커
    /// RGB 값 직접 입력도 지원하게 추가!
    /// 색이 바뀔때마다 OnColorChanged 이벤트로 알린다!
    /// </summary>
    public class ColorPickerUI : MonoBehaviour
    {
        [Header("Color Wheel (Hue + Saturation)")]
        [SerializeField] private RawImage wheelImage;
        [SerializeField] private RectTransform wheelHandle;
        [SerializeField] private ColorWheelDragHandler wheelDrag;
        
        [Header("Value")]
        [SerializeField] private RawImage valueStripImage;
        [SerializeField] private Slider valueSlider;
        
        [Header("RGB Input")]
        [SerializeField] private TMP_InputField rInput;
        [SerializeField] private TMP_InputField gInput;
        [SerializeField] private TMP_InputField bInput;
        
        [Header("Settings")]
        [SerializeField] private int wheelTextureSize = 200;
        [SerializeField] private int stripTextureSize = 64;
        
        public Action<Color> OnColorChanged;
        
        private Texture2D wheelTexture;
        private Texture2D valueTexture;

        private float hue = 0f;
        private float saturation = 0f;
        private float value = 1f;
        
        private void Awake()
        {
            BuildWheelTexture();
            
            valueTexture = new Texture2D(stripTextureSize, 1, TextureFormat.RGB24, false);
            valueTexture.wrapMode = TextureWrapMode.Clamp;
            valueStripImage.texture = valueTexture;
            RefreshValueTexture();
            
            valueSlider.onValueChanged.AddListener(OnValueChanged);
            wheelDrag.OnDragPolar = OnWheelChanged;
            
            rInput.onEndEdit.AddListener(OnRGBInputChanged);
            gInput.onEndEdit.AddListener(OnRGBInputChanged);
            bInput.onEndEdit.AddListener(OnRGBInputChanged);
            
            UpdateRGBInputs();
        }

        private void BuildWheelTexture()
        {
            wheelTexture = new Texture2D(wheelTextureSize, wheelTextureSize, TextureFormat.RGBA32, false);
            wheelTexture.wrapMode = TextureWrapMode.Clamp;

            float radius = wheelTextureSize / 2f;
            Color32[] pixels = new Color32[wheelTextureSize * wheelTextureSize];

            for (int y = 0; y < wheelTextureSize; y++)
            {
                float dy = y + 0.5f - radius;
                for (int x = 0; x < wheelTextureSize; x++)
                {
                    float dx = x + 0.5f - radius;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy) / radius;

                    if (dist > 1f)
                    {
                        pixels[y * wheelTextureSize + x] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    float angle = Mathf.Atan2(dy, dx);
                    float h = angle / (Mathf.PI * 2f);
                    if (h < 0f) h += 1f;
                    
                    pixels[y * wheelTextureSize + x] = Color.HSVToRGB(h, dist, 1f);
                }
            }
            
            wheelTexture.SetPixels32(pixels);
            wheelTexture.Apply();
            
            wheelImage.texture = wheelTexture;
        }

        private void RefreshValueTexture()
        {
            for (int x = 0; x < stripTextureSize; x++)
            {
                float v = x / (float)(stripTextureSize - 1);
                valueTexture.SetPixel(x, 0, Color.HSVToRGB(hue, saturation, v));
            }
            valueTexture.Apply();
        }

        /// <summary>
        /// 휠 드래그 콜백: 각도 -> Hue(0~1), 반지름 -> Saturation(0~1) 
        /// </summary>
        private void OnWheelChanged(float wheelHue, float wheelSaturation)
        {
            hue = wheelHue;
            saturation = wheelSaturation;
            
            UpdateWheelHandlePosition();
            RefreshValueTexture();
            UpdateRGBInputs();
            ApplyColor();
        }

        private void OnValueChanged(float sliderValue)
        {
            value = sliderValue;
            UpdateRGBInputs();
            ApplyColor();
        }


        /// <summary>
        /// R/G/B 입력창 값이 확정(엔터/포커스 아웃)되면 휠/슬라이더에 반영한다.
        /// </summary>
        private void OnRGBInputChanged(string _)
        {
            int r = ParseByte(rInput.text);
            int g = ParseByte(gInput.text);
            int b = ParseByte(bInput.text);
            
            Color color = new Color32((byte)r, (byte)g, (byte)b, 255);
            Color.RGBToHSV(color, out hue, out saturation, out value);
            
            valueSlider.SetValueWithoutNotify(value);
            UpdateWheelHandlePosition();
            RefreshValueTexture();
            UpdateRGBInputs();
            ApplyColor();
        }

        private int ParseByte(string text)
        {
            int result;
            if (!int.TryParse(text, out result)) result = 0;
            return Mathf.Clamp(result, 0, 255);
        }
        
        /// <summary>
        /// 현재 색을 R/G/B 입력창 텍스트에 반영한다.
        /// </summary>
        private void UpdateRGBInputs()
        {
            Color32 c32 = Color.HSVToRGB(hue, saturation, value);
            rInput.text = c32.r.ToString();
            gInput.text = c32.g.ToString();
            bInput.text = c32.b.ToString();
        }

        private void UpdateWheelHandlePosition()
        {
            float radius = wheelImage.rectTransform.rect.width / 2f;
            float angle = hue * Mathf.PI * 2f;
            float r = saturation * radius;
            
            wheelHandle.anchoredPosition = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
        }
        
        private void ApplyColor()
        {
            Color color = Color.HSVToRGB(hue, saturation, value);
            OnColorChanged?.Invoke(color);
        }
        
        /// <summary>
        /// 외부에서 특정 색으로 초기화 한다 (에빈트는 발생시키지 않음)
        /// </summary>
        public void SetColor(Color color)
        {
            Color.RGBToHSV(color, out hue, out saturation, out value);
            
            valueSlider.SetValueWithoutNotify(value);
            UpdateWheelHandlePosition();
            RefreshValueTexture();
            UpdateRGBInputs();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FiveInARow.Controls   //名称空间需要修改！
{
    /// <summary>
    /// FButton.xaml 的交互逻辑
    /// </summary>
    public partial class FButton : Button
    {
        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register(
            "PressedBackground", typeof (Brush), typeof (FButton), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// 鼠标按下背景样式
        /// </summary>
        public Brush PressedBackground
        {
            get { return (Brush) GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedForegroundProperty = DependencyProperty.Register(
            "PressedForeground", typeof (Brush), typeof (FButton), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// 鼠标按下前景样式（图标、文字）
        /// </summary>
        public Brush PressedForeground
        {
            get { return (Brush) GetValue(PressedForegroundProperty); }
            set { SetValue(PressedForegroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register(
            "MouseOverBackground", typeof (Brush), typeof (FButton), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// 鼠标进入背景样式
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush) GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register(
            "MouseOverForeground", typeof (Brush), typeof (FButton), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// 鼠标进入前景样式
        /// </summary>
        public Brush MouseOverForeground
        {
            get { return (Brush) GetValue(MouseOverForegroundProperty); }
            set { SetValue(MouseOverForegroundProperty, value); }
        }

        public static readonly DependencyProperty FIconProperty = DependencyProperty.Register(
            "FIcon", typeof (string), typeof (FButton), new PropertyMetadata(default(string)));
        /// <summary>
        /// 按钮字体图标编码
        /// </summary>
        public string FIcon
        {
            get { return (string) GetValue(FIconProperty); }
            set { SetValue(FIconProperty, value); }
        }

        public static readonly DependencyProperty FIconSizeProperty = DependencyProperty.Register(
            "FIconSize", typeof (int), typeof (FButton), new PropertyMetadata(default(int)));
        /// <summary>
        /// 按钮图标大小
        /// </summary>
        public int FIconSize
        {
            get { return (int) GetValue(FIconSizeProperty); }
            set { SetValue(FIconSizeProperty, value); }
        }

        public static readonly DependencyProperty FIconMarginProperty = DependencyProperty.Register(
            "FIconMargin", typeof (Thickness), typeof (FButton), new PropertyMetadata(default(Thickness)));
        /// <summary>
        /// 字体图标间距
        /// </summary>
        public Thickness FIconMargin
        {
            get { return (Thickness) GetValue(FIconMarginProperty); }
            set { SetValue(FIconMarginProperty, value); }
        }

        public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.Register(
            "AllowsAnimation", typeof (bool), typeof (FButton), new PropertyMetadata(default(bool)));
        /// <summary>
        /// 是否启用FIcon动画
        /// </summary>
        public bool AllowsAnimation
        {
            get { return (bool) GetValue(AllowsAnimationProperty); }
            set { SetValue(AllowsAnimationProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof (CornerRadius), typeof (FButton), new PropertyMetadata(default(CornerRadius)));
        /// <summary>
        /// 按钮圆角大小，左上，右上，右下，左下
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty ContentDecorationsProperty = DependencyProperty.Register(
            "ContentDecorations", typeof (TextDecorationCollection), typeof (FButton), new PropertyMetadata(default(TextDecorationCollection)));

        public TextDecorationCollection ContentDecorations
        {
            get { return (TextDecorationCollection) GetValue(ContentDecorationsProperty); }
            set { SetValue(ContentDecorationsProperty, value); }
        }

        static FButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FButton),new FrameworkPropertyMetadata(typeof(FButton)));
        }
    }//End public class
}//End namespace

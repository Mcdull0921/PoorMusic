using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace HZH_Controls
{
    public class ImagePropertyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            //指定为模式窗体属性编辑器类型
            return UITypeEditorEditStyle.Modal;
        }

    }
}

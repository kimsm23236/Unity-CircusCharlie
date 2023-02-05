using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static partial class GFunc
{
    //! �ؽ�Ʈ�޽����� ������ ������Ʈ�� �ؽ�Ʈ�� �����ϴ� �Լ�
    public static void SetTmpText(this GameObject obj_, string text_)
    {
        TMP_Text tmpTxt = obj_.GetComponentInChildren<TMP_Text>();
        if (tmpTxt == null || tmpTxt == default(TMP_Text))
        {
            return;
        }       // if: ������ �ؽ�Ʈ�޽� ������Ʈ�� ���� ���

        // ������ �ؽ�Ʈ�޽� ������Ʈ�� �����ϴ� ���
        tmpTxt.text = text_;
        
    }       // SetTextMeshPro()
}

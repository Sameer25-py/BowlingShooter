using System;
using TMPro;
using UnityEngine;

public class MultiText : MonoBehaviour
{
    private TMP_Text _text;
    public  string   EnglishString;
    public  string   RussianString;

    public void RenderText(Language selectedLanguage)
    {
        _text = GetComponent<TMP_Text>();
        switch (selectedLanguage)
        {
            case Language.English:
                _text.text = EnglishString;
                break;
            case Language.Russian:
                _text.text = RussianString;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(selectedLanguage), selectedLanguage, null);
        }
    }
}
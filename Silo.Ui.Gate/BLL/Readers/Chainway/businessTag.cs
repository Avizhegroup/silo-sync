using Silo.Ui.Gate.Models;

namespace Silo.Ui.Gate.BLL;
public  class businessTag
{
    internal System.Collections.ArrayList GetEPCTagReadedTag(List<Tags> ListTagReaded, int BeforSecond)
    {
        System.Collections.ArrayList EPCResult = new System.Collections.ArrayList();
        DateTime BaseTime = DateTime.Now.AddSeconds(-BeforSecond);
        foreach (Tags _tag in ListTagReaded)
        {
            if (_tag.TagReedTime >= BaseTime)
            {
                if (!EPCResult.Contains(_tag.TagEPC))
                    EPCResult.Add(_tag.TagEPC);
            }
        }

        return EPCResult;
    }
    
}

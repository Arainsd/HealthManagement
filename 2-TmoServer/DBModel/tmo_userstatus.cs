using System;
using System.Text;
using System.Collections.Generic;
namespace DBModel
{
    [Serializable]
    public class tmo_userstatus
    {
        public string id { get; set; }
        /// <summary>
        /// �û����
        /// </summary>		
        public string user_id { get; set; }
        /// <summary>
        /// �ʾ����
        /// </summary>		
        public int usertimes { get; set; }
        /// <summary>
        /// �ʾ���дʱ��
        /// </summary>		
        public DateTime questionnaire_time { get; set; }
        /// <summary>
        /// �ʾ�״̬ 0-�ݴ� 1-���ύ(�ȴ�����) 2-������
        /// </summary>		
        public short questionnare_status { get; set; }
        /// <summary>
        /// �û�ѡ����ʾ����
        /// </summary>
        public string qc_ids { get; set; }
        /// <summary>
        /// �ʾ�����ʱ��
        /// </summary>		
        public DateTime assessment_time { get; set; }

    }
}
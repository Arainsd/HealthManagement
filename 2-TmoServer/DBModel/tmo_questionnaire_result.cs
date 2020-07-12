using System;

namespace DBModel
{
    [Serializable]
    public class tmo_questionnaire_result
    {
        /// <summary>
        /// qr_id
        /// </summary>		
        public string qr_id { get; set; }
        /// <summary>
        /// q_id
        /// </summary>		
        public string q_id { get; set; }
        /// <summary>
        /// user_id
        /// </summary>		
        public string user_id { get; set; }
        /// <summary>
        /// qr_result
        /// </summary>		
        public string qr_result { get; set; }
        /// <summary>
        /// qr_score
        /// </summary>		
        public float qr_score { get; set; }
        /// <summary>
        /// �Ƿ񴥷�Σ������
        /// </summary>
        public byte qr_is_risk { get; set; }
        /// <summary>
        /// user_times
        /// </summary>		
        public int user_times { get; set; }
        /// <summary>
        /// creater
        /// </summary>		
        public string creater { get; set; }
        /// <summary>
        /// input_time
        /// </summary>		
        public DateTime input_time { get; set; }
        /// <summary>
        /// ��ǰ�û�״̬
        /// </summary>
        public tmo_userstatus Userstatus { get; set; }
    }
}
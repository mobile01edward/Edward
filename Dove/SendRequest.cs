using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Xml;

namespace Dove
{
    class SendRequest
    {
        public void NewRequest(string HR, AsyncCallback responseCallback)
        {
            //Uri RestUri = new Uri(HR + "&output=json");
            try
            {
                //创建WebRequest类
                //HttpWebRequest request = HttpWebRequest.CreateHttp(RestUri);
                HttpWebRequest request = HttpWebRequest.CreateHttp(new Uri(HR + "&output=json")) as HttpWebRequest;

                //设置请求方式GET POST
                request.Method = "GET";

                //返回应答请求异步操作的状态
                request.BeginGetResponse(responseCallback, request);
                //request.BeginGetResponse(responseCallback, request);
            }
            catch (WebException e)
            {
                //网络相关异常处理
            }
            catch (Exception e)
            {
                //异常处理
            }
        }

        public void responseCall(IAsyncResult result)
        {
            try
            {
                //获取异步操作返回的的信息
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                //结束对 Internet 资源的异步请求
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                //解析应答头
                //parseRecvHeader(response.Headers);
                //获取请求体信息长度
                //long contentLength = response.ContentLength;

                //获取应答码
                //int statusCode = (int)response.StatusCode;
                //string statusText = response.StatusDescription;

                using (Stream stream = response.GetResponseStream())
                {
                    //获取请求信息
                    StreamReader read = new StreamReader(stream);
                    string msg = read.ReadToEnd();
                    //UpdateNewFeed(msg);
                    App.MyReceiveString = msg;

                    response.Close();
                    response.Dispose();
                }
            }
            catch (WebException e)
            {
                //连接失败
            }
            catch (Exception e)
            {
                //异常处理
            }
        }

        public void UpdateNewFeed(string Feed)
        {
            byte[] data = Encoding.Unicode.GetBytes(Feed);
            MemoryStream memStream = new MemoryStream(data);
            var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(RootObject));

            RootObject readObject = (RootObject)dataContractJsonSerializer.ReadObject(memStream);

            int count = readObject.Mylist.Count;
           
        }

        string post_text = "";
        public void SendPost(string url,string PostString)
        {
            //var url = "http://posttestserver.com/post.php";
            post_text = PostString;

            // Create the web request object
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // Start the request
            webRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallbacksend), webRequest);
        }

        void GetRequestStreamCallbacksend(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the stream request operation
            Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);

            // Create the post data
            // Demo POST data 

            string postData = string.Format("post_text={0}", post_text);

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            // Start the web request
            webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallbacksend), webRequest);
        }

        void GetResponseCallbacksend(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response;

                // End the get response operation
                response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);

                using (XmlReader reader2 = XmlReader.Create(streamReader))
                {
                    //SyndicationFeed feed = SyndicationFeed.Load(reader);
                    //int i = 0;

                    while (reader2.Read())
                    {
                        // Only detect start elements.
                        if (reader2.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader2.Name)
                            {
                                case "forum_id":
                                    if (reader2.Read())
                                    {
                                        Debug.WriteLine("forum_id={0}", reader2.Value.Trim());
                                    }
                                    break;
                                case "topic_id":
                                    if (reader2.Read())
                                    {
                                        Debug.WriteLine("topic_id={0}", reader2.Value.Trim());
                                    }
                                    break;
                            }
                        }
                    }

                }
                var Response = streamReader.ReadToEnd();
                streamResponse.Close();
                streamReader.Close();
                response.Close();
            }
            catch (WebException e)
            {
                // Error treatment
                // ...
            }
        }
    }

    public class RootObject
    {
        public List<FleetsCollection> Mylist { get; set; }
    }

    public class FleetsCollection
    {
        public string topic_id { get; set; }
        public string topic_title { get; set; }
        public string topic_time { get; set; }
        public string topic_views { get; set; }
        public string topic_replies { get; set; }
        public string topic_status { get; set; }
        public string topic_notify { get; set; }
        public string topic_sticky { get; set; }
        public string topic_poster_uid { get; set; }
        public string topic_poster_uname { get; set; }
        public string topic_poster_icon { get; set; }
        public string topic_desc { get; set; }
        public string favorite_status { get; set; }
        public string forum_name { get; set; }
        public string forum_id { get; set; }
    }
}

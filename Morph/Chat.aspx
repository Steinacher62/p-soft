<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="ch.appl.psoft.Morph.Chat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function hasUserMedia() {
            navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia
                || navigator.mozGetUserMedia || navigator.msGetUserMedia;
            return !!navigator.getUserMedia;
        }

        if (hasUserMedia()) {
            navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia
                || navigator.mozGetUserMedia || navigator.msGetUserMedia;

            //get both video and audio streams from user's camera 

            navigator.mediaDevices.getUserMedia({ video: true, audio: true })
                .then(function (stream) {
                    var video = document.querySelector('video');
                    //insert stream into the video tag 
                    $('video')[0].src = window.URL.createObjectURL(stream);
                })
                .catch(function (err) {
                    /* handle the error */
                });
        } else {
            alert("Error. WebRTC is not supported!");
        }
    </script>

     <telerik:RadMediaPlayer ID="VideoPlayer" runat="server"></telerik:RadMediaPlayer>

</asp:Content>

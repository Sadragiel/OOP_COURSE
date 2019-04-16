var WebTextToSpeech = {
	CheckAvailable : function(){
		return "speechSynthesis" in window;
	},
	PlayText : function(text){
		if("speechSynthesis" in window){
			function Say(_text){
				var msg = new SpeechSynthesisUtterance(Pointer_stringify(_text));
				window.speechSynthesis.speak(msg);
			}
			
			setTimeout(Say,10,text);
		}
	}
};

mergeInto(LibraryManager.library,WebTextToSpeech);
var tl = fl.getDocumentDOM().getTimeline();
fl.outputPanel.clear();

var out='Animation2D anim = (Animation2D)this.GetComponent(typeof(Animation2D));\n';
out+='Element2D element;\n\n'

var elementCount=0;

for(var i=tl.layers.length-1;i>=0;i--) {
	var frames = tl.layers[i].frames;
	var el=frames[0].elements[0];
	if(el!=null) {
		elementCount++;
	}
}

out+='anim.Initialize('+document.frameRate+', '+tl.frameCount+', '+elementCount+');\n\n';
for(var i=tl.layers.length-1;i>=0;i--) {
	var frames = tl.layers[i].frames;
	var n = frames.length;
	var keyframes=0;
	
	for (j=0; j<n; j++) {
		if (j==frames[j].startFrame) {
			keyframes++;
		}
	}
	
	var el=frames[0].elements[0];
	if(el!=null) {
		if(el.name[0]=='_') {
			out+='element = new Element2D(anim,"'+
				el.name.substr(1,el.name.length-1)+'", '+
				el.x+'f, '+
				(-el.y)+'f, '+
				el.scaleX+'f, '+
				el.scaleY+'f, '+
				(-el.rotation)+'f, '+
				keyframes+
			');\n';
		} else {
			var item=el.libraryItem;
			var mask=(item.timeline.layers[0].frames[0].elements[0]);
			var bitmap=(item.timeline.layers[1].frames[0].elements[0]);
			var x1=(mask.x-bitmap.x)/bitmap.width;
			var y1=1-((mask.y-bitmap.y))/bitmap.height;
			var x2=(mask.x-bitmap.x+mask.width)/bitmap.width;
			var y2=1-(mask.y-bitmap.y+mask.height)/bitmap.height;
			
			out+='element = new Element2D(anim,"'+
				el.name+'", '+
				el.x+'f, '+
				(-el.y)+'f, '+
				el.scaleX+'f, '+
				el.scaleY+'f, '+
				(-el.rotation)+'f, '+
				mask.x+'f, '+
				(-mask.y)+'f, '+
				(mask.x+mask.width)+'f, '+
				(-(mask.y+mask.height))+'f, '+
				x1+'f, '+
				y1+'f, '+
				x2+'f, '+
				y2+'f, '+
				(el.colorRedPercent/100)+'f, '+
				(el.colorGreenPercent/100)+'f, '+
				(el.colorBluePercent/100)+'f, '+
				(el.colorAlphaPercent/100)+'f, '+
				keyframes+
			');\n';
		}
	}
	
	

	for (j=0; j<n; j++) {
		if (j==frames[j].startFrame) {
			if(frames[j].labelType!='none') {
				out+='anim.AddLabel('+j+',"'+frames[j].name+'");\n'
			}
			var el2=frames[j].elements[0];
			if(el2!=null) {
				out+='element.AddKeyframe(new Keyframe2D('+
					j+', '+
					el2.x+'f, '+
					(-el2.y)+'f, '+
					el2.scaleX+'f, '+
					el2.scaleY+'f, '+
					(-el2.rotation)+'f, '+
					(el2.colorRedPercent/100)+'f, '+
					(el2.colorGreenPercent/100)+'f, '+
					(el2.colorBluePercent/100)+'f, '+
					(el2.colorAlphaPercent/100)+'f'+
				'));';
				out+='\n';
			}
		}
	}
	if(el!=null) out+='anim.AddElement(element);\n\n';
}

fl.trace(out);
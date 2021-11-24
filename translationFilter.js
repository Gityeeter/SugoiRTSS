module.exports = function filterText(extractedText) {
	let result = extractedText

	result = result.replace(/{/g, "") // remove { sign in all positions of the text
	result = result.replace("�", ""); // remove the first unknown sign in the text
        result = result.replace(/カ$/, ""); // remove カ symbol at the end of the text
	result = result.replace(/987$/, "?"); // replace number 987 at the end of the text with ? 
	result = result.replace(/^:/, ""); // remove colon at the beginning of the text
	result = result.replace(/^:/, ""); // remove colon at the beginning of the text
	
	const fs = require('fs');

	let text1 = { 
	    ttext: result	  
	};
 
	let data = JSON.stringify(text1);
	fs.writeFileSync('text_temp.json', data);
	return result
}

// to filter a text that only show up at the end, add the $ sign at the end. Example /goodbye$/ will remove "goodbye" from "James, goodbye"
// to filter a text that only show up at the beginning, add the ^ sign at the beginning. Example /^Hello/ will remove "Hello" from "Hello, James" 
// to filter a text only once at any position, it's easy. Example "James" will remove "James" from "I helped James today"
// to filter a text in all position, use the g symbol. Example "/apple/g" will remove "apple" from "I hate apple but like apple Iphone"

// if you need more help, contact me in the Discord group
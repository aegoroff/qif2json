parser grammar Qif2json;

options { tokenVocab=Qif2jsonLexer; }

compileUnit
	: typeIdentifier transactionList
	;

typeIdentifier : TYPE_START type LINE_BREAK;

type : TYPE ;

transactionList 
	: TRAN_START LINE_BREAK transactionList
	| line+
	;

line : code literal_string (LINE_BREAK | EOF) ;

code : LINE_START ;

literal_string : TEXT;

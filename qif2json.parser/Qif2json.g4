parser grammar Qif2json;

options { tokenVocab=Qif2jsonLexer; }

compileUnit
	: startTransaction transaction*
	;

startTransaction : typeIdentifier line+ endTransaction ;

typeIdentifier : TYPE_MARKER (TYPE_START type | account) LINE_BREAK;
endTransaction : END_TRANSACTION (LINE_BREAK | EOF) ;

type : TYPE ;
account : ACCOUNT ;

transaction 
	: line+ endTransaction
	;

line : code literal_string LINE_BREAK ;

code : LINE_START ;

literal_string : TEXT;

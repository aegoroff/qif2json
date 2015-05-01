// Created by: egr
// Created at: 30.04.2015
// QIF file format parser grammar
// © 2015 Alexander Egorov

parser grammar Qif2json;

options { tokenVocab=Qif2jsonLexer; }

compileUnit
	: account+
	;

account
	: startTransaction transaction*
	;

startTransaction 
	: header line+ endTransaction 
	;

header 
	: TYPE_MARKER (TYPE_START type | accountType) LINE_BREAK
	;

endTransaction 
	: END_TRANSACTION (LINE_BREAK | EOF) 
	;

transaction 
	: line+ endTransaction
	;

line 
	: code literal_string 
	;

type : TYPE ;

accountType : ACCOUNT ;

code : LINE_START ;

literal_string : LITERAL ;

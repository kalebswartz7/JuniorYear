import java.io.*;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;
import java.util.StringTokenizer;

//Java Interpreter of made up language z+-.
//Author: Kaleb Swartz

public class Main {
	
	public static Scanner readFile;
	public static String fileName;
	public static Map<String, String> stringMap = new HashMap<String, String>();
	public static long startTime;

	
	public static void main(String[] args) throws IOException {
		startTime = System.nanoTime();
		fileName = args[0];
		openFile();
		readFile();
		
	}
	
	public static void openFile() {
		try {
			readFile = new Scanner(new File(fileName));
		}
		catch(Exception e) {
			System.out.println("File not found");
		}
		
	}
	
	public static void readFile() {
		while(readFile.hasNext()) {
			String a = "";
			
			a += readFile.nextLine().replaceAll("\\s", "");
			checkString(a);
		}
	}
	
	public static void checkString(String line) {
		boolean removeSpaces = true;
		for (int j = 0; j < line.length(); j++) {
			String a = line.substring(j, j+1);
			if (a.equals("\"")) {
				removeSpaces = false;
			}
		}
		int lineNum = 1;
		for (int i = 0; i < line.length() - 1; i++) {
			lineNum++;
			char a = line.charAt(i);
			char b = line.charAt(i+1);
			if (Character.isLetter(a)) {
				if (b == '=') {
					AssignmentStatement(line);
					return;
					
				}
				else if (b == 'R') {
					printVariable(line);
					return;
				}
				else if (a == 'F' && b == 'O') {
					forLoop(line, lineNum);
					return;
				}
		}
				else if (a == '*') {
					multiplyVariable(line, lineNum);
					return;
				}
				else if (a == '-') {
					subVariable(line, lineNum);
					return;
				}
				else if (a == '+') {
					addVariable(line, lineNum);
					return;
				}
		}
			
	}
	
	
	public static void AssignmentStatement(String line) {
		boolean quotes = false;
		String numberValue = "";
		String stringValue = "";
		String assign = "";
		for (int i = 0; i < line.length(); i++) {
			char a = line.charAt(i);
			if (i == 0) assign += a;
			if (a == '=') {
				for (int j = i+1; j < line.length(); j++) {
					char b = line.charAt(j);
					if (b == '"') quotes = true;
					String b_ = String.valueOf(b);
					if (b != ';') {
						if (Character.isLetter(b)) {
							if (stringMap.containsKey(b_) && !quotes) {
								stringValue += stringMap.get(b_);
							}
							else {
								stringValue += b;

							}
						}
						else {
							if (b != '"') numberValue += b;
						}
					}
				}
			}
		}
		if (numberValue.length() > stringValue.length()) {
			String insert = numberValue;
			stringMap.put(assign, insert);
		}
		else {
			String insert = stringValue;
			stringMap.put(assign, insert);
		}
		
		
	}
	
	public static void printMap(HashMap m) {
		String a = m.toString();
		String b = "";
		for (int i = 0; i < a.length()-1; i++) {
			if (!a.substring(i, i+1).equals("{") && !a.substring(i, i+1).equals("}")) {
				b += a.substring(i, i+1);
			}
		}
		System.out.print(b);
	}
	
	public static void printVariable(String line) {
		String value = "";
		for (int i = 0; i < line.length(); i++) {
			if (line.substring(i, i+1).equals("T")) {
				value += line.substring(i+1, i+2);
				System.out.println(value + " = " + stringMap.get(value));
				i = line.length();
			}
		}
		
	}
	
	public static void multiplyVariable(String line, int lineNum) {
		String multiplied = line.substring(0, 1);
		String checking = stringMap.get(multiplied);
		char realCheck = checking.charAt(0);
		if (!Character.isDigit(realCheck)) {
				System.out.println("Runtime error at line " + lineNum + ". Cannot multiply string and variable together.");
				return;
			
		}
		String multiplier = "";
		for (int i = 0; i < line.length(); i++) {			   
			if (line.substring(i, i+1).equals("=")) {
				for (int j = i + 1; j < line.length(); j++) {
					if (!line.substring(j, j+1).equals(";")) {
						multiplier += line.substring(j, j+1);
					}
				}
			}
		}
		char isLetter = multiplier.charAt(0);
		if (Character.isLetter(isLetter)) {
			String newValue = stringMap.get(multiplier);
			char check = newValue.charAt(0);
			if (Character.isLetter(check)) {
				System.out.println("Runtime error at line " + lineNum + ". Cannot multiply string and variable together.");
				return;
			}
			
			int a = Integer.parseInt(newValue);
			int b = Integer.parseInt(stringMap.get(multiplied));
			int c = a * b;
			newValue = "";
			newValue += c;
			stringMap.put(multiplied, newValue);
		}
		else {
			if (isLetter == '"') {
				System.out.println("Runtime error at line " + lineNum + ". Cannot multiply string and variable together.");
				return;
			}
			String newValue = "";
			int a = Integer.parseInt(stringMap.get(multiplied));
			int b = Integer.parseInt(multiplier);
			int c = a*b;
			newValue += c;
			stringMap.put(multiplied, newValue);
		}
		
	}
	
	public static  void subVariable(String line, int lineNum) {
		String added = line.substring(0, 1);
		String newAddition = "";
		for (int i = 0; i < line.length(); i++) {
			if (line.substring(i, i+1).equals("=")) {
				for (int j = i + 1; j < line.length(); j++) {
					if (!line.substring(j, j+1).equals(";")) {
						newAddition += line.substring(j, j+1);
					}
				}
			}
		}
		char isLetter = newAddition.charAt(0);
		if (Character.isLetter(isLetter)) {
			String newValue = stringMap.get(newAddition);
			char check = newValue.charAt(0);
			String checker = stringMap.get(added);
			char check2 = checker.charAt(0);
			if (Character.isLetter(check) || Character.isLetter(check2)) {
				System.out.println("Runtime error at line " + lineNum + ". Cannot subtract string and variable values.");
				return;
			}
			int a = Integer.parseInt(newValue);
			int b = Integer.parseInt(stringMap.get(added));
			int c = b - a;
			newValue = "";
			newValue += c;
			stringMap.put(added, newValue);
		}
		else {
			char isLetter2 = newAddition.charAt(0);
			String isLetter3 = stringMap.get(added);
			char isLetter4 = isLetter3.charAt(0);
			if (isLetter2 == '"' || !Character.isDigit(isLetter4)) {
				System.out.println("Runtime error at line " + lineNum + ". Cannot subtract string and variable values.");
				return;
			}
			String newValue = "";
			int a = Integer.parseInt(stringMap.get(added));
			int b = Integer.parseInt(newAddition);
			int c = a - b;
			newValue += c;
			stringMap.put(added, newValue);
		}
	}
	
	public static void addVariable(String line, int lineNum) {
		boolean addingString = false; 
		boolean addingVariable = false;
		String key = line.substring(0,1);
		String newString = "";
		String beingAdded = "";
		for (int i = 0; i < line.length(); i++) {
			char a = line.charAt(i);
			if (a == '"') {
				addingString = true;
			}
			else if (a == ';') {
				char b = line.charAt(i-1);
				if (Character.isLetter(b)) {
					addingVariable = true;
				}
				
			}
		}
		if (addingString) {
			String check = line.substring(0,1);
			String value = stringMap.get(check);
			char check2 = value.charAt(0);
			if (Character.isDigit(check2)) {
				System.out.println("Error on line " + lineNum + ". Cannot add int and string together.");
				return;
			}
			for (int i = 0; i < line.length(); i++) {
				if (line.substring(i, i+1).equals("\"")) {
					for (int j = i+1; j < line.length(); j++) {
						if (!line.substring(j, j+1).equals(";") && !line.substring(j, j+1).equals("\"")) {
							beingAdded += line.substring(j, j+1);
						}
					}
					
				}
			}
			String oldString = stringMap.get(key);
			newString = oldString + " " + beingAdded;
			stringMap.put(key, newString);
		}
		else if (addingVariable) {
			String addingVariable1 = "";
			String oldKey = line.substring(0,1);
			for (int i = 0; i < line.length(); i++) {
				if (line.substring(i, i+1).equals(";")) {
					addingVariable1 += line.substring(i-1, i);
				}
			}
			String addingValue = stringMap.get(addingVariable1);
			char check = addingValue.charAt(0);
			if (!Character.isDigit(check)) {
				String anotherCheck = stringMap.get(oldKey);
				char check2 = anotherCheck.charAt(0);
				if (Character.isDigit(check2)) {
					System.out.println("Error on line " + lineNum + ". Cannot add int and string together.");
					return;
				}
				String newestString = "";
				newestString += stringMap.get(oldKey);
				newestString += " " + addingValue;
				stringMap.put(oldKey, newestString);
			}
			else {
				String checker = stringMap.get(oldKey);
				char check2 = checker.charAt(0);
				if (!Character.isDigit(check2)) {
					System.out.println("Error on line " + lineNum + ". Cannot add int and string together.");
					return;
				}
				String addition = stringMap.get(addingVariable1);
				int x = Integer.parseInt(addition);
				int y = Integer.parseInt(stringMap.get(oldKey));
				int z = x + y;
				String finalString = "";
				finalString += z;
				stringMap.put(oldKey, finalString);
			}
			
		}
		else {
			String firstValue = line.substring(0, 1);
			String realValue = stringMap.get(firstValue);
			char check = realValue.charAt(0);
			if (!Character.isDigit(check)) {
				System.out.println("Error on line " + lineNum + ". Cannot add int and string together.");
				return;
			}
			String newValue = "";
			for (int i = 0; i < line.length(); i++) {
				if (line.substring(i, i+1).equals("=")) {
					for (int j = i + 1; j < line.length(); j++) {
						if (!line.substring(j, j+1).equals(";")) {
							newValue += line.substring(j, j+1);
						}
					}
				}
			}
			int x = Integer.parseInt(realValue);
			int y = Integer.parseInt(newValue);
			int z = x + y;
			String z2 = "";
			z2 += z;
			stringMap.put(firstValue, z2);

		}
		
	}
	
	public static void forLoop(String line, int lineNum) {
		String count = "";
		String action = "";
		int loops;
		for (int i = 0; i < line.length(); i++) {
			if (line.substring(i, i+1).equals("R")) {
				count += line.substring(i+1, i+2);
				i = line.length();
			}
		}
		loops = Integer.parseInt(count);
		for (int i = 0; i < line.length(); i++) {
			if (line.substring(i, i+1).equals(count)) {
				for (int j = i+1; j < line.length(); j++) {
					if (!line.substring(j, j+1).equals("E")) {
						action += line.substring(j, j+1);
					}
					else {
						j = line.length();
					}
				}
				
			}
		}
		
		String sendString = "";
		String eraseString = "";
		for (int k = 0; k < loops; k++) {
			for (int i = 0; i < action.length(); i++) {
				if (!action.substring(i,  i+1).equals(";")) {
					sendString += action.substring(i, i+1);
				}
				else {
					sendString += ";";
					checkString(sendString);
					sendString = eraseString;
				}
			}
		}
		
		
	}

	
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class LogicalLineUtils
{
    public static class Encapsulation
    {
        private static char ENCAPSULATION_START = '{';
        private static char ENCAPSULATION_END = '}';
        public class EncapsulatedData
        {
            public bool IsNull() { return lines == null; }
            private List<string> lines;
            private int startingIndex;
            private int endingIndex;
            public void Iniciate(int startingIndex)
            {
                this.startingIndex = startingIndex;
                lines = new List<string>();
                endingIndex = 0;
            }
            public List<string> GetLines() { return lines; }

            public void SetEndingIndex(int endingIndex) { this.endingIndex = endingIndex; }
            public int GetEndingIndex() { return endingIndex; }
            public void SetStartingIndex(int startingIndex) { this.startingIndex = startingIndex; }
            public int GetStartingIndex() { return startingIndex; }
        }
        
        public static EncapsulatedData RipEncapsulatedData(Conversation convesation, int startIndex, bool ripHeadEncap = false)
        {
            int encpsulationDepth = 0;
            EncapsulatedData data = new EncapsulatedData();
            data.Iniciate(startIndex);
            for (int i = startIndex; i < convesation.Count(); ++i)
            {
                string line = convesation.GetLines()[i];
                if (ripHeadEncap || (encpsulationDepth > 0 && !IsEncapsulatingEnd(line)))
                {
                    data.GetLines().Add(line);
                }
                if (IsEncapsulatingStart(line))
                {
                    ++encpsulationDepth;
                    continue;
                }
                if (IsEncapsulatingEnd(line))
                {
                    --encpsulationDepth;
                    if (encpsulationDepth == 0)
                    {
                        data.SetEndingIndex(i);
                        break;
                    }
                }
            }
            return data;
        }

        public static bool IsEncapsulatingStart(string line) { return line.Trim().StartsWith(ENCAPSULATION_START); }
        public static bool IsEncapsulatingEnd(string line) { return line.Trim().StartsWith(ENCAPSULATION_END); }
    }

    public static class Expressions
    {
        public static HashSet<string> OPERATORS() { return new HashSet<string>() { "+", "+=", "-", "-=", "*", "*=", "/", "/=", "=" }; }
        public static string REGEX_ARITMATIC() {return @"([-+*/=]=?)";}
        public static string REGEX_OPERATOR_LINE() { return @"^\$\w+\s*(=|\+=|-=|\*=\/=|)\s*"; }

        public static object CalculateValue(string[] expresPart)
        {
            List<string> operadString= new List<string>();
            List<string> operatorString= new List<string>();
            List<object> operands = new List<object>();

            for (int i = 0; i < expresPart.Length; ++i)
            {
                string part = expresPart[i].Trim();
                if (part == string.Empty)
                {
                    continue;
                }
                if (OPERATORS().Contains(part))
                { operatorString.Add(part); }
                else
                { operadString.Add(part); }
            }
            foreach (string part in operadString) 
            {
                operands.Add(ExtractValue(part));
            }
            CalculaDivMul(operatorString, operands);
            CalculaSumRes(operatorString, operands);
            Debug.Log(operands.Count);
            return operands[0];
        }
        private static void CalculaDivMul(List<string> operatorStrings, List<object> operands)
        {
            for (int i = 0;i < operatorStrings.Count; ++i) 
            {
                string operatorString = operatorStrings[i];

                if (operatorString =="*" || operatorString == "/")
                {
                    double leftOp= Convert.ToDouble(operands[i]);
                    double rightOp= Convert.ToDouble(operands[i+1]);

                    if (operatorString == "*")
                    {
                        operands[i] = leftOp * rightOp;
                    }
                    else
                    {
                        if (rightOp == 0)
                        {
                            if (rightOp == 0)
                            {
                                Debug.LogError("cannot divide by zero");
                                return;
                            }
                            operands[i] = leftOp / rightOp;
                        }
                    }
                    operands.RemoveAt(i + 1);
                    operatorStrings.RemoveAt(i);
                    --i;
                }
            }
        }
        private static void CalculaSumRes(List<string> operatorStrings, List<object> operands)
        {
            for (int i = 0; i < operatorStrings.Count; ++i)
            {
                string operatorString = operatorStrings[i];

                if (operatorString == "+" || operatorString == "-")
                {
                    double leftOp = Convert.ToDouble(operands[i]);
                    double rightOp = Convert.ToDouble(operands[i + 1]);

                    if (operatorString == "+")
                    {
                        operands[i] = leftOp + rightOp;
                    }
                    else
                    {
                        operands[i] = leftOp - rightOp;
                    }
                    operands.RemoveAt(i + 1);
                    operatorStrings.RemoveAt(i);
                    --i;
                }
            }

        }

        private static object ExtractValue(string value)
        {
            bool negate = false;
            if (value.StartsWith('!'))
            {
                negate = true;
                value = value.Substring(1);
            }
            if (value.StartsWith(VariableStore.VARIABLE_ID()))
            {
                string variableName = value.TrimStart(VariableStore.VARIABLE_ID());
                if(!VariableStore.HasVarable(variableName))
                {
                    Debug.LogError($"Varieble {variableName} does not exist !");
                    return null;
                }
                VariableStore.TryGetValue(variableName, out object val);

                if(val is bool boolVal && negate)
                {
                    return !boolVal;
                }
                return val;
            }
            else if(value.StartsWith('\"') && value.EndsWith('\"'))
            {
                value = TagManager.Inject(value , true, true);
                return value.Trim('"');
            }
            else
            {
                if(int.TryParse(value,out int intValue))
                { return intValue; }
                
                else if(float.TryParse(value, out float floatValue))
                { return floatValue; }
                
                else if (bool.TryParse(value, out bool boolValue))
                { return negate ? !boolValue : boolValue; }
                
                else 
                {
                    value = TagManager.Inject(value, true, true);
                    return value; 
                }
            }
        }
    }

    public static class Conditions
    {
        public static string REGEX_CONDITIONAL_OPERATORS() { return @"(==|!=|<=|>=|<|>|&&|\|\|)"; }
        public static bool EvaluateCondition(string condition)
        {
            condition= TagManager.Inject(condition, injeTag :true ,injectVar: true);

            string[] parts = Regex.Split(condition, REGEX_CONDITIONAL_OPERATORS())
                .Select(p => p.Trim()).ToArray();
            for(int i =0; i< parts.Length; ++i) 
            {
                if (parts[i].StartsWith("\"")&& parts[i].EndsWith("\""))
                {
                    parts[i] = parts[i].Substring(1, parts[i].Length-2);
                }
            }
            if(parts.Length ==1)
            {
                if(bool.TryParse(parts[0], out bool result))
                {
                    return result;
                }
                else
                {
                    Debug.LogError($"Couldnot parse condition: {condition}");
                    return false;
                }
            }
            else if (parts.Length ==3)
            {
                return EvaluateExpresion(parts[0], parts[1], parts[2]);
            }
            else
            {
                Debug.LogError($"Unsopurted condition format: {condition}");
                return false;
            }
        }
        //T tipo generico
        private delegate bool OperatorFunc<T>(T left, T right);
        private static Dictionary<string,OperatorFunc<bool>> boolOperators = new Dictionary<string, OperatorFunc<bool>>()
        {
            { "&&", (left,right) => left && right},
            { "||", (left,right) => left || right},
            { "==", (left,right) => left == right},
            { "!=", (left,right) => left != right}
        };
        private static Dictionary<string,OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>()
        {
            { "==", (left,right) => left == right},
            { "!=", (left,right) => left != right},
            { ">", (left,right) => left > right},
            { ">=" , (left, right) => left >= right},
            { "<" , (left, right) => left < right},
            { "<=" , (left, right) => left <= right}
        };
        private static Dictionary<string, OperatorFunc<float>> intOperators = new Dictionary<string, OperatorFunc<float>>()
        {
            { "==", (left,right) => left == right},
            { "!=", (left,right) => left != right},
            { ">", (left,right) => left > right},
            { ">=" , (left, right) => left >= right},
            { "<" , (left, right) => left < right},
            { "<=" , (left, right) => left <= right}
        };
        private static bool EvaluateExpresion(string left,string op, string right)
        {
            if (bool.TryParse(left, out bool leftBool) && bool.TryParse(right, out bool rightBool))
            {
                if (boolOperators.ContainsKey(op))
                {
                    return boolOperators[op](leftBool, rightBool);
                }
            }
            if (float.TryParse(left, out float leftFloat) && float.TryParse(right, out float rightFloat))
            {
                
                if (floatOperators.ContainsKey(op))
                {
                    return floatOperators[op](leftFloat, rightFloat);
                }
            }
            if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
            {
                if (intOperators.ContainsKey(op))
                {
                    return intOperators[op](leftInt, rightInt);
                }
            }
            switch (op)
            {
                case "==":return left == right;
                case "!=":return left != right;
                default: throw new InvalidOperationException($"Unsuported Operation: {op}");
            }
        }
    }
}

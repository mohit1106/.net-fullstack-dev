// using System;
// class Program{
//     static void Main(){
//         // int[][] jag = new int[2][];
//         // jag[0] = new int[]{1,2};
//         // jag[1] = new int[]{3,4,5};

//         // Console.WriteLine(jag[1][2]);
//         // for(int i=0; i<jag.Length; i++){
//         //     for(int j=0; j<jag[i].Length; j++){
//         //         Console.WriteLine(jag[i][j]);
//         //     }
//         // }
        
//         // int[] marks = {5,4,9,8,9};
//         // Array.Resize(ref marks, 4);
//         // // Array.Clear(marks, 2, marks.Length-1);
//         // for(int j=0; j<marks.Length; j++){
//         //         Console.WriteLine(marks[j]);
//         // }
//         // Console.WriteLine(Array.Exists(marks, x => x > 8));


//         // copy arrays
//         // int[] masks = new int[5];
//         // Array.Copy(marks, masks, 4);
//         // // Array.Copy(marks, masks, 2);
//         // for(int j=0; j<masks.Length; j++){
//         //     Console.Write(masks[j] + " ");
//         // }

//         // List<int> num1 = new List<int>();
//         // num1.add(5);
//         // num1.add(10);

//         // ArrayList num2 = new ArrayList();
//         // num2.add(5);
//         // num2.add(10);

//         // for(int j=0; j<num2.Size(); j++){
//         //     Console.Write(num2.get(j));
//         // }


//         // HashSet<int> set = new HashSet<int>();
//         // set.Add(5);
//         // set.Add(5);
//         // set.Add(6);
//         // foreach(int a in set){
//         //     Console.WriteLine(a);
//         // }


//         // SortedList<String, String> list = new SortedList<String, String>();
//         // list.Add("b", "B");
//         // list.Add("a", "A");
        

//         int[] arr = {1,2,3,2,1,4,2};
//         Dictionary<int, int> dict = new Dictionary<int, int>();
//         foreach(int a in arr){
//             if(dict.ContainsKey(a)) dict[a]++;
//             else dict[a] = 1;
//         }
//         foreach(KeyValuePair<int, int> a in dict){
//             Console.WriteLine(a.Key + " - " + a.Value);
//         }


//         int[] arr1 = {1, 3, 5};
//         int[] arr2 = {2, 4, 6};
//         int[] arr = new int[arr1.Length + arr2.Length];
//         int i =0, j=0, k=0;
//         while(i<arr1.Length && j <arr2.Length){
//             if(arr1[i] < arr2[j]){
//                 arr[k++] = arr1[i++];
//             }
//             else if(arr1[i] > arr2[j]){
//                 arr[k++] = arr2[j++];
//             }
//         }
//         while(i<arr1.Length) arr[k++] = arr1[i++];
//         while(j<arr2.Length) arr[k++] = arr2[j++];

//         for(int x=0; x<arr.Length; x++){
//             Console.Write(arr[x] + " ");
//         }
//     }
// }





using System;

class Program
{
    public static string CleanseAndInvert(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 6) return "";

        foreach (char c in input)
        {
            if (!char.IsLetter(c)) return "";
        }
        input = input.ToLower();

        string filtered = "";
        foreach (char c in input)
        {
            if ((int)c % 2 != 0)
            {
                filtered += c;
            }
        }

        char[] arr = filtered.ToCharArray();
        Array.Reverse(arr);

        for (int i = 0; i < arr.Length; i++)
        {
            if (i % 2 == 0)
            {
                arr[i] = char.ToUpper(arr[i]);
            }
        }
        return new string(arr);
    }

    static void Main()
    {
        Console.WriteLine("Enter the word");
        string input = Console.ReadLine();

        string result = CleanseAndInvert(input);

        if (result == "")
        {
            Console.WriteLine("Invalid Input");
        }
        else
        {
            Console.WriteLine("The generated key is - " + result);
        }
    }
}

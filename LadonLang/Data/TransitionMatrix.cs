namespace LadonLang.Data
{
    public struct TransitionMatrix
    {
        private static readonly int[][] matrix = [
            [0,1,2,0,-5,5,7,41,1,10,59,17,51,35,1,19,65,21,1,44,48,1,1,54],
            [100,1,1,100,100,-4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [200,-1,2,200,3,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1],
            [-2,-2,4,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2],
            [300,-2,4,300,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2],
            [5,5,5,5,5,6,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5],
            [400,-3,-3,5,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3],
            [100,1,1,100,100,-4,1,8,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-4,1,1,9,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [500,1,1,500,500,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,11,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,12,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,13,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,14,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,15,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,16,1,1,1,1,1,1,1,1,1],
            [600,1,1,600,600,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,18,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [700,1,1,700,700,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,20,1,1,1,1,1,1,1],
            [800,1,1,800,800,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,23,1,1,1,22,1,1,32,1,1,1,1,1,1,1,1,1],
            [900,1,1,900,900,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,24,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,25,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,26,1,1,1,1,1],
            [1000,1,1,1000,1000,-3,1,1,1,1,1,27,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,28,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,29,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,30,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,31,1,1,1,1,1,1,1,1,1],
            [1100,1,1,1100,1100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,33,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,34,1,1,1,1],
            [1200,1,1,1200,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,36,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,37,1,1,1,1,1,1,1,1,39,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,38,1,1,1,1,1,1,1,1],
            [1300,1,1,1300,1300,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,40,1,1],
            [1400,1,1,1400,1400,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,42,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,43,1],
            [1500,1,1,1500,1500,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,45,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,46,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,47,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1600,1,1,1600,1600,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,49,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,50,1,1,1,1],
            [1700,1,1,1700,1700,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,52,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,53,1,1,1,1,1,1,1,1,1,1],
            [1800,1,1,1800,1800,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,55,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,56,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,57,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,58,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1900,1,1,1900,1900,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,60,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,61,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,1,1,1,62,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,63,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,64,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [2000,1,1,2000,2000,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,66,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,100,-3,1,1,1,1,1,1,1,1,67,1,1,1,1,1,1,1,1,1],
            [2100,1,1,2100,2100,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]
        ];

        public static int[][] Matrix { get => matrix; }//set => matrix = value;
    }
}

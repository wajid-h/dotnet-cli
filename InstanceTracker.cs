

namespace  CLI {
    public  class InstanceTracker {

        
        public static Dictionary<Type , object> Instances {get =>  instances ; }
        private  static readonly Dictionary<Type , object> instances = [];

        public static void  AddInstance (Type  ofType, object targetInstance) { 
            
            instances.Remove(ofType) ;
            instances.Add(ofType , targetInstance) ; 

        }   
        public static bool  TryAddInstance(Type  ofType, object targetInstance) {
            return instances.TryAdd(ofType , targetInstance) ;
        }   

        
        public static void RemoveInstance(Type ofType) {
            instances.Remove(ofType) ;
        }   


        public static object? GetInstance(Type ofType) {
                                                                                        
            instances.TryGetValue(ofType , out object? inst) ;
            if(inst != null) return inst;
            if(!Settings.ForceInstantiatedTypes) return  inst ; 
            inst =  Activator.CreateInstance(ofType);
            return  inst ;
        }
    }
}
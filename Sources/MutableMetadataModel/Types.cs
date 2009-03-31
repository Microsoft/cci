//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci.MutableCodeModel {

  public abstract class AliasForType : IAliasForType, ICopyFrom<IAliasForType> {

    internal AliasForType() {
      this.aliasedType = Dummy.TypeReference;
      this.attributes = new List<ICustomAttribute>();
      this.locations = new List<ILocation>();
      this.members = new List<IAliasMember>();
    }

    public void Copy(IAliasForType aliasForType, IInternFactory internFactory) {
      this.aliasedType = aliasForType.AliasedType;
      this.attributes = new List<ICustomAttribute>(aliasForType.Attributes);
      this.locations = new List<ILocation>(aliasForType.Locations);
      this.members = new List<IAliasMember>(aliasForType.Members);
    }

    public ITypeReference AliasedType {
      get { return this.aliasedType; }
      set { this.aliasedType = value; }
    }
    ITypeReference aliasedType;

    public List<ICustomAttribute> Attributes {
      get { return this.attributes; }
      set { this.attributes = value; }
    }
    List<ICustomAttribute> attributes;

    //^ [Pure]
    public bool Contains(IAliasMember member) {
      foreach (IAliasMember tdmem in this.Members)
        if (member == tdmem) return true;
      return false;
    }

    public abstract void Dispatch(IMetadataVisitor visitor);

    //^ [Pure]
    public IEnumerable<IAliasMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<IAliasMember, bool> predicate) {
      foreach (IAliasMember tdmem in this.Members) {
        if (tdmem.Name.UniqueKey == name.UniqueKey || ignoreCase && (name.UniqueKeyIgnoringCase == tdmem.Name.UniqueKeyIgnoringCase)) {
          if (predicate(tdmem)) yield return tdmem;
        }
      }
    }

    //^ [Pure]
    public IEnumerable<IAliasMember> GetMatchingMembers(Function<IAliasMember, bool> predicate) {
      foreach (IAliasMember tdmem in this.Members) {
        if (predicate(tdmem)) yield return tdmem;
      }
    }

    //^ [Pure]
    public IEnumerable<IAliasMember> GetMembersNamed(IName name, bool ignoreCase) {
      foreach (IAliasMember tdmem in this.Members) {
        if (tdmem.Name.UniqueKey == name.UniqueKey || ignoreCase && (name.UniqueKeyIgnoringCase == tdmem.Name.UniqueKeyIgnoringCase)) {
          yield return tdmem;
        }
      }
    }

    public List<ILocation> Locations {
      get { return this.locations; }
      set { this.locations = value; }
    }
    List<ILocation> locations;

    public List<IAliasMember> Members {
      get { return this.members; }
      set { this.members = value; }
    }
    List<IAliasMember> members;

    #region IAliasForType

    IEnumerable<IAliasMember> IAliasForType.Members {
      get { return this.members.AsReadOnly(); }
    }

    #endregion

    #region IReference Members

    IEnumerable<ICustomAttribute> IReference.Attributes {
      get { return this.attributes.AsReadOnly(); }
    }

    IEnumerable<ILocation> IReference.Locations {
      get { return this.locations.AsReadOnly(); }
    }

    #endregion

    #region IContainer<IAliasMember> Members

    IEnumerable<IAliasMember> IContainer<IAliasMember>.Members {
      get { return this.members.AsReadOnly(); }
    }

    #endregion

    #region IScope<IAliasMember> Members

    IEnumerable<IAliasMember> IScope<IAliasMember>.Members {
      get { return this.members.AsReadOnly(); }
    }

    #endregion

  }

  public sealed class CustomModifier : ICustomModifier, ICopyFrom<ICustomModifier> {

    public CustomModifier() {
      this.isOptional = false;
      this.modifier = Dummy.TypeReference;
    }

    public void Copy(ICustomModifier customModifier, IInternFactory internFactory) {
      this.isOptional = customModifier.IsOptional;
      this.modifier = customModifier.Modifier;
    }

    public bool IsOptional {
      get { return this.isOptional; }
      set { this.isOptional = value; }
    }
    bool isOptional;

    public ITypeReference Modifier {
      get { return this.modifier; }
      set { this.modifier = value; }
    }
    ITypeReference modifier;

  }

  public sealed class FunctionPointerTypeReference : TypeReference, IFunctionPointerTypeReference, ICopyFrom<IFunctionPointerTypeReference> {

    public FunctionPointerTypeReference() {
      this.callingConvention = (CallingConvention)0;
      this.extraArgumentTypes = new List<IParameterTypeInformation>();
      this.parameters = new List<IParameterTypeInformation>();
      this.returnValueCustomModifiers = new List<ICustomModifier>();
      this.returnValueIsByRef = false;
      this.type = Dummy.TypeReference;
    }

    public void Copy(IFunctionPointerTypeReference functionPointerTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(functionPointerTypeReference, internFactory);
      this.callingConvention = functionPointerTypeReference.CallingConvention;
      this.extraArgumentTypes = new List<IParameterTypeInformation>(functionPointerTypeReference.ExtraArgumentTypes);
      this.parameters = new List<IParameterTypeInformation>(functionPointerTypeReference.Parameters);
      if (functionPointerTypeReference.ReturnValueIsModified)
        this.returnValueCustomModifiers = new List<ICustomModifier>(functionPointerTypeReference.ReturnValueCustomModifiers);
      else
        this.returnValueCustomModifiers = new List<ICustomModifier>(0);
      this.returnValueIsByRef = functionPointerTypeReference.ReturnValueIsByRef;
      this.type = functionPointerTypeReference.Type;
    }

    public CallingConvention CallingConvention {
      get { return this.callingConvention; }
      set { this.callingConvention = value; }
    }
    CallingConvention callingConvention;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public List<IParameterTypeInformation> ExtraArgumentTypes {
      get { return this.extraArgumentTypes; }
      set { this.extraArgumentTypes = value; }
    }
    List<IParameterTypeInformation> extraArgumentTypes;

    public List<IParameterTypeInformation> Parameters {
      get { return this.parameters; }
      set { this.parameters = value; }
    }
    List<IParameterTypeInformation> parameters;

    public override ITypeDefinition ResolvedType {
      get { 
        return new FunctionPointerType(this.callingConvention, this.returnValueIsByRef, this.type, this.returnValueCustomModifiers.AsReadOnly(), this.parameters.AsReadOnly(),
        this.extraArgumentTypes.AsReadOnly(), this.InternFactory); 
      }
    }

    public List<ICustomModifier> ReturnValueCustomModifiers {
      get { return this.returnValueCustomModifiers; }
      set { this.returnValueCustomModifiers = value; }
    }
    List<ICustomModifier> returnValueCustomModifiers;

    public bool ReturnValueIsByRef {
      get { return this.returnValueIsByRef; }
      set { this.returnValueIsByRef = value; }
    }
    bool returnValueIsByRef;

    public bool ReturnValueIsModified {
      get { return this.returnValueCustomModifiers.Count > 0; }
    }

    public ITypeReference Type {
      get { return this.type; }
      set { this.type = value; }
    }
    ITypeReference type;

    #region IFunctionPointerTypeReference Members

    IEnumerable<IParameterTypeInformation> IFunctionPointerTypeReference.ExtraArgumentTypes {
      get { return this.extraArgumentTypes.AsReadOnly(); }
    }

    #endregion

    #region ISignature Members


    IEnumerable<IParameterTypeInformation> ISignature.Parameters {
      get { return this.parameters.AsReadOnly(); }
    }

    IEnumerable<ICustomModifier> ISignature.ReturnValueCustomModifiers {
      get { return this.returnValueCustomModifiers.AsReadOnly(); }
    }

    #endregion
  }

  public sealed class GenericMethodParameterReference : TypeReference, IGenericMethodParameterReference, ICopyFrom<IGenericMethodParameterReference> {

    public GenericMethodParameterReference() {
      this.definingMethod = Dummy.MethodReference;
      this.name = Dummy.Name;
      this.index = 0;
    }

    public void Copy(IGenericMethodParameterReference genericMethodParameterReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(genericMethodParameterReference, internFactory);
      this.definingMethod = genericMethodParameterReference.DefiningMethod;
      this.name = genericMethodParameterReference.Name;
      this.index = genericMethodParameterReference.Index;
    }

    public IMethodReference DefiningMethod {
      get { return this.definingMethod; }
      set { this.definingMethod = value; }
    }
    IMethodReference definingMethod;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public IName Name {
      get { return this.name; }
      set { this.name = value; }
    }
    IName name;

    public ushort Index {
      get { return this.index; }
      set { this.index = value; }
    }
    ushort index;

    private IGenericMethodParameter Resolve() {
      IMethodDefinition definingMethod = this.definingMethod.ResolvedMethod;
      if (definingMethod.IsGeneric && definingMethod.GenericParameterCount > this.index) {
        ushort i = 0;
        foreach (IGenericMethodParameter par in definingMethod.GenericParameters) {
          if (par.Index == i++) return par;
        }
      }
      return Dummy.GenericMethodParameter;
    }

    IGenericMethodParameter IGenericMethodParameterReference.ResolvedType {
      get {
        if (this.resolvedType == null)
          this.resolvedType = this.Resolve();
        return this.resolvedType;
      }
    }
    IGenericMethodParameter/*?*/ resolvedType;

    public override ITypeDefinition ResolvedType {
      get { return ((IGenericMethodParameterReference)this).ResolvedType; }
    }

  }

  public abstract class GenericParameter : TypeDefinition, IGenericParameter, ICopyFrom<IGenericParameter> {

    //^ [NotDelayed]
    internal GenericParameter() {
      this.constraints = new List<ITypeReference>();
      this.index = 0;
      //^ base;
      this.MustBeReferenceType = false;
      this.MustBeValueType = false;
      this.MustHaveDefaultConstructor = false;
      this.Variance = TypeParameterVariance.NonVariant;
    }

    public void Copy(IGenericParameter genericParameter, IInternFactory internFactory) {
      ((ICopyFrom<INamedTypeDefinition>)this).Copy(genericParameter, internFactory);
      this.constraints = new List<ITypeReference>(genericParameter.Constraints);
      this.index = genericParameter.Index;
      this.MustBeReferenceType = genericParameter.MustBeReferenceType;
      this.MustBeValueType = genericParameter.MustBeValueType;
      this.MustHaveDefaultConstructor = genericParameter.MustHaveDefaultConstructor;
      this.Variance = genericParameter.Variance;
    }

    public List<ITypeReference> Constraints {
      get { return this.constraints; }
      set { this.constraints = value; }
    }
    List<ITypeReference> constraints;

    private ITypeDefinition GetEffectiveBaseClass() {
      ITypeDefinition mostDerivedBaseClass = this.PlatformType.SystemObject.ResolvedType;
      foreach (ITypeReference constraint in this.Constraints) {
        ITypeDefinition constraintType = constraint.ResolvedType;
        if (constraintType.IsClass && TypeHelper.Type1DerivesFromType2(constraintType, mostDerivedBaseClass))
          mostDerivedBaseClass = constraintType;
      }
      return mostDerivedBaseClass;
    }

    public ushort Index {
      get { return this.index; }
      set { this.index = value; }
    }
    ushort index;

    public override bool IsReferenceType {
      get {
        if (((int)this.flags & 0x00000800) == 0) {
          this.flags |= (TypeDefinition.Flags)0x00000800;
          if (this.MustBeReferenceType)
            this.flags |= (TypeDefinition.Flags)0x00000400;
          else {
            ITypeDefinition baseClass = this.GetEffectiveBaseClass();
            if (!TypeHelper.TypesAreEquivalent(baseClass, this.PlatformType.SystemObject) && baseClass != Dummy.Type) {
              if (baseClass.IsClass)
                this.flags |= (TypeDefinition.Flags)0x00000400;
              else if (baseClass.IsValueType)
                this.flags |= (TypeDefinition.Flags)0x00000200;
            }
          }
        }
        return ((int)this.flags & 0x00000400) != 0;
      }
    }

    public override bool IsValueType {
      get {
        if (((int)this.flags & 0x00000800) == 0) {
          this.flags |= (TypeDefinition.Flags)0x00000800;
          if (this.MustBeReferenceType)
            this.flags |= (TypeDefinition.Flags)0x00000400;
          else {
            ITypeDefinition baseClass = this.GetEffectiveBaseClass();
            if (!TypeHelper.TypesAreEquivalent(baseClass, this.PlatformType.SystemObject) && baseClass != Dummy.Type) {
              if (baseClass.IsClass)
                this.flags |= (TypeDefinition.Flags)0x00000400;
              else if (baseClass.IsValueType)
                this.flags |= (TypeDefinition.Flags)0x00000200;
            }
          }
        }
        return ((int)this.flags & 0x00000200) != 0;
      }
    }

    public bool MustBeReferenceType {
      get { return (this.flags & TypeDefinition.Flags.MustBeReferenceType) != 0; }
      set
        //^ requires value ==> !this.MustBeValueType;
      {
        if (value)
          this.flags |= TypeDefinition.Flags.MustBeReferenceType; 
        else
          this.flags &= ~TypeDefinition.Flags.MustBeReferenceType;
      }
    }

    public bool MustBeValueType {
      get { return (this.flags & TypeDefinition.Flags.MustBeValueType) != 0; }
      set
        //^ requires value ==> !this.MustBeReferenceType;
      { 
        if (value)
          this.flags |= TypeDefinition.Flags.MustBeValueType; 
        else
          this.flags &= ~TypeDefinition.Flags.MustBeValueType;
      }
    }

    public bool MustHaveDefaultConstructor {
      get { return (this.flags & TypeDefinition.Flags.MustHaveDefaultConstructor) != 0; }
      set { 
        if (value)
          this.flags |= TypeDefinition.Flags.MustHaveDefaultConstructor; 
        else
          this.flags &= ~TypeDefinition.Flags.MustHaveDefaultConstructor;
      }
    }

    public TypeParameterVariance Variance {
      get { return (TypeParameterVariance)((int)this.flags>>4) & TypeParameterVariance.Mask; }
      set { 
        this.flags &= (TypeDefinition.Flags)~((int)TypeParameterVariance.Mask<<4);
        this.flags |= (TypeDefinition.Flags)((int)(value&TypeParameterVariance.Mask)<<4);
      }
    }

    #region IGenericParameter Members

    IEnumerable<ITypeReference> IGenericParameter.Constraints {
      get { return this.constraints.AsReadOnly(); }
    }

    #endregion
  }

  public sealed class GenericTypeInstanceReference : TypeReference, IGenericTypeInstanceReference, ICopyFrom<IGenericTypeInstanceReference> {

    public GenericTypeInstanceReference() {
      this.genericArguments = new List<ITypeReference>();
      this.genericType = Dummy.TypeReference;
    }

    public void Copy(IGenericTypeInstanceReference genericTypeInstanceReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(genericTypeInstanceReference, internFactory);
      this.genericArguments = new List<ITypeReference>(genericTypeInstanceReference.GenericArguments);
      this.genericType = genericTypeInstanceReference.GenericType;
    }

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public List<ITypeReference> GenericArguments {
      get { return this.genericArguments; }
      set { this.genericArguments = value; }
    }
    List<ITypeReference> genericArguments;

    public ITypeReference GenericType {
      get { return this.genericType; }
      set { this.genericType = value; }
    }
    ITypeReference genericType;

    IGenericTypeInstance ResolvedGenericTypeInstance {
      get {
        return GenericTypeInstance.GetGenericTypeInstance(this.genericType, this.genericArguments.AsReadOnly(), this.InternFactory);
      }
    }

    public override ITypeDefinition ResolvedType {
      get { return this.ResolvedGenericTypeInstance; }
    }

    #region IGenericTypeInstanceReference Members

    IEnumerable<ITypeReference> IGenericTypeInstanceReference.GenericArguments {
      get { return this.genericArguments.AsReadOnly(); }
    }

    #endregion

  }

  public sealed class GenericTypeParameter : GenericParameter, IGenericTypeParameter, ICopyFrom<IGenericTypeParameter> {

    //^ [NotDelayed]
    public GenericTypeParameter() {
      this.definingType = Dummy.Type;
      //^ base;
    }

    public void Copy(IGenericTypeParameter genericTypeParameter, IInternFactory internFactory) {
      ((ICopyFrom<IGenericParameter>)this).Copy(genericTypeParameter, internFactory);
      this.definingType = genericTypeParameter.DefiningType;
    }

    public ITypeDefinition DefiningType {
      get { return this.definingType; }
      set { this.definingType = value; }
    }
    ITypeDefinition definingType;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    #region IGenericTypeParameterReference Members

    ITypeReference IGenericTypeParameterReference.DefiningType {
      get { return this.DefiningType; }
    }

    IGenericTypeParameter IGenericTypeParameterReference.ResolvedType {
      get { return this; }
    }

    #endregion
  }

  public sealed class GenericTypeParameterReference : TypeReference, IGenericTypeParameterReference, ICopyFrom<IGenericTypeParameterReference> {

    public GenericTypeParameterReference() {
      this.definingType = Dummy.TypeReference;
      this.name = Dummy.Name;
      this.index = 0;
    }

    public void Copy(IGenericTypeParameterReference genericTypeParameterReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(genericTypeParameterReference, internFactory);
      this.definingType = genericTypeParameterReference.DefiningType;
      this.name = genericTypeParameterReference.Name;
      this.index = genericTypeParameterReference.Index;
    }

    public ITypeReference DefiningType {
      get { return this.definingType; }
      set { this.definingType = value; }
    }
    ITypeReference definingType;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public IName Name {
      get { return this.name; }
      set { this.name = value; }
    }
    IName name;

    public ushort Index {
      get { return this.index; }
      set { this.index = value; }
    }
    ushort index;

    private IGenericTypeParameter Resolve() {
      ITypeDefinition definingType = this.definingType.ResolvedType;
      if (definingType.IsGeneric && definingType.GenericParameterCount > this.index) {
        ushort i = 0;
        foreach (IGenericTypeParameter par in definingType.GenericParameters) {
          if (par.Index == i++) return par;
        }
      }
      return Dummy.GenericTypeParameter;
    }

    IGenericTypeParameter IGenericTypeParameterReference.ResolvedType {
      get {
        if (this.resolvedType == null)
          this.resolvedType = this.Resolve();
        return this.resolvedType;
      }
    }
    IGenericTypeParameter/*?*/ resolvedType;

    public override ITypeDefinition ResolvedType {
      get { return ((IGenericTypeParameterReference)this).ResolvedType; }
    }

  }

  public sealed class MatrixTypeReference : TypeReference, IArrayTypeReference, ICopyFrom<IArrayTypeReference> {

    public MatrixTypeReference() {
      this.elementType = Dummy.Type;
      this.lowerBounds = new List<int>();
      this.rank = 0;
      this.sizes = new List<ulong>();
    }

    public void Copy(IArrayTypeReference matrixTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(matrixTypeReference, internFactory);
      this.elementType = matrixTypeReference.ElementType;
      this.lowerBounds = new List<int>(matrixTypeReference.LowerBounds);
      this.rank = matrixTypeReference.Rank;
      this.sizes = new List<ulong>(matrixTypeReference.Sizes);
    }

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public ITypeReference ElementType {
      get { return this.elementType; }
      set { this.elementType = value; }
    }
    ITypeReference elementType;

    public bool IsVector {
      get { return false; }
    }

    public List<int> LowerBounds {
      get { return this.lowerBounds; }
      set { this.lowerBounds = value; }
    }
    List<int> lowerBounds;

    public uint Rank {
      get { return this.rank; }
      set { this.rank = value; }
    }
    uint rank;

    public List<ulong> Sizes {
      get { return this.sizes; }
      set { this.sizes = value; }
    }
    List<ulong> sizes;

    IArrayType ResolvedArrayType {
      get {
        return Matrix.GetMatrix(this.ElementType, this.Rank, this.lowerBounds.AsReadOnly(), this.sizes.AsReadOnly(), this.InternFactory);
      }
    }

    public override ITypeDefinition ResolvedType {
      get { return this.ResolvedArrayType; }
    }


    #region IArrayTypeReference Members


    IEnumerable<int> IArrayTypeReference.LowerBounds {
      get { return this.lowerBounds.AsReadOnly(); }
    }

    IEnumerable<ulong> IArrayTypeReference.Sizes {
      get { return this.sizes.AsReadOnly(); }
    }

    #endregion

  }

  public sealed class MethodImplementation : IMethodImplementation, ICopyFrom<IMethodImplementation> {

    public MethodImplementation() {
      this.containingType = Dummy.Type;
      this.implementedMethod = Dummy.MethodReference;
      this.implementingMethod = Dummy.MethodReference;
    }

    public void Copy(IMethodImplementation methodImplementation, IInternFactory internFactory) {
      this.containingType = methodImplementation.ContainingType;
      this.implementedMethod = methodImplementation.ImplementedMethod;
      this.implementingMethod = methodImplementation.ImplementingMethod;
    }

    public ITypeDefinition ContainingType {
      get { return this.containingType; }
      set { this.containingType = value; }
    }
    ITypeDefinition containingType;

    public void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public IMethodReference ImplementedMethod {
      get { return this.implementedMethod; }
      set { this.implementedMethod = value; }
    }
    IMethodReference implementedMethod;

    public IMethodReference ImplementingMethod {
      get { return this.implementingMethod; }
      set { this.implementingMethod = value; }
    }
    IMethodReference implementingMethod;

  }

  public sealed class NamespaceAliasForType : AliasForType, INamespaceAliasForType, ICopyFrom<INamespaceAliasForType> {

    public NamespaceAliasForType() {
      this.containingNamespace = Dummy.RootUnitNamespace;
      this.isPublic = false;
      this.name = Dummy.Name;
    }

    public void Copy(INamespaceAliasForType namespaceAliasForType, IInternFactory internFactory) {
      ((ICopyFrom<IAliasForType>)this).Copy(namespaceAliasForType, internFactory);
      this.containingNamespace = namespaceAliasForType.ContainingNamespace;
      this.isPublic = namespaceAliasForType.IsPublic;
      this.name = namespaceAliasForType.Name;
    }

    public INamespaceDefinition ContainingNamespace {
      get { return this.containingNamespace; }
      set { this.containingNamespace = value; }
    }
    INamespaceDefinition containingNamespace;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public bool IsPublic {
      get { return this.isPublic; }
      set { this.isPublic = value; }
    }
    bool isPublic;


    public IName Name {
      get { return this.name; }
      set { this.name = value; }
    }
    IName name;

    INamespaceDefinition IContainerMember<INamespaceDefinition>.Container {
      get { return this.ContainingNamespace; }
    }

    IScope<INamespaceMember> IScopeMember<IScope<INamespaceMember>>.ContainingScope {
      get { return this.ContainingNamespace; }
    }

  }

  public sealed class NamespaceTypeDefinition : TypeDefinition, INamespaceTypeDefinition, ICopyFrom<INamespaceTypeDefinition> {

    public NamespaceTypeDefinition() {
      this.containingUnitNamespace = Dummy.RootUnitNamespace;
    }

    public void Copy(INamespaceTypeDefinition namespaceTypeDefinition, IInternFactory internFactory) {
      ((ICopyFrom<INamedTypeDefinition>)this).Copy(namespaceTypeDefinition, internFactory);
      this.containingUnitNamespace = namespaceTypeDefinition.ContainingUnitNamespace;
      this.IsPublic = namespaceTypeDefinition.IsPublic;
    }

    public IUnitNamespace ContainingUnitNamespace {
      get { return containingUnitNamespace; }
      set { this.containingUnitNamespace = value; }
    }
    IUnitNamespace containingUnitNamespace;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public bool IsPublic {
      get {
        return (((TypeMemberVisibility)this.flags) & TypeMemberVisibility.Mask) == TypeMemberVisibility.Public;
      }
      set {
        this.flags &= (TypeDefinition.Flags)~TypeMemberVisibility.Mask;
        if (value)
          this.flags |= (TypeDefinition.Flags)TypeMemberVisibility.Public;
      }
    }

    #region INamespaceMember Members

    INamespaceDefinition INamespaceMember.ContainingNamespace {
      get { return this.ContainingUnitNamespace; }
    }

    #endregion

    #region IContainerMember<INamespaceDefinition> Members

    INamespaceDefinition IContainerMember<INamespaceDefinition>.Container {
      get { return this.ContainingUnitNamespace; }
    }

    #endregion

    #region IScopeMember<IScope<INamespaceMember>> Members

    IScope<INamespaceMember> IScopeMember<IScope<INamespaceMember>>.ContainingScope {
      get { return this.ContainingUnitNamespace; }
    }

    #endregion

    #region INamespaceTypeReference Members

    IUnitNamespaceReference INamespaceTypeReference.ContainingUnitNamespace {
      get { return this.ContainingUnitNamespace; }
    }

    INamespaceTypeDefinition INamespaceTypeReference.ResolvedType {
      get { return this; }
    }

    #endregion
  }

  public sealed class NamespaceTypeReference : TypeReference, INamespaceTypeReference, ICopyFrom<INamespaceTypeReference> {

    public NamespaceTypeReference() {
      this.containingUnitNamespace = Dummy.RootUnitNamespace;
      this.genericParameterCount = 0;
      this.mangleName = true;
      this.name = Dummy.Name;
    }

    public void Copy(INamespaceTypeReference namespaceTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(namespaceTypeReference, internFactory);
      this.containingUnitNamespace = namespaceTypeReference.ContainingUnitNamespace;
      this.genericParameterCount = namespaceTypeReference.GenericParameterCount;
      this.mangleName = namespaceTypeReference.MangleName;
      this.name = namespaceTypeReference.Name;
    }

    public IUnitNamespaceReference ContainingUnitNamespace {
      get { return this.containingUnitNamespace; }
      set { this.containingUnitNamespace = value; this.resolvedType = null; }
    }
    IUnitNamespaceReference containingUnitNamespace;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public ushort GenericParameterCount {
      get { return this.genericParameterCount; }
      set { this.genericParameterCount = value; this.resolvedType = null; }
    }
    ushort genericParameterCount;

    private INamespaceTypeDefinition Resolve() {
      foreach (INamespaceMember member in this.containingUnitNamespace.ResolvedUnitNamespace.GetMembersNamed(this.name, false)) {
        INamespaceTypeDefinition/*?*/ nsType = member as INamespaceTypeDefinition;
        if (nsType != null && nsType.GenericParameterCount == this.genericParameterCount) return nsType;
      }
      return Dummy.NamespaceTypeDefinition;
    }

    public override ITypeDefinition ResolvedType {
      get { return ((INamespaceTypeReference)this).ResolvedType; }
    }

    INamedTypeDefinition INamedTypeReference.ResolvedType {
      get { return ((INamespaceTypeReference)this).ResolvedType; }
    }

    INamespaceTypeDefinition INamespaceTypeReference.ResolvedType {
      get {
        if (this.resolvedType == null)
          this.resolvedType = this.Resolve();
        return this.resolvedType;
      }
    }
    INamespaceTypeDefinition/*?*/ resolvedType;

    public bool MangleName {
      get { return this.mangleName; }
      set { this.mangleName = value; }
    }
    bool mangleName;

    public IName Name {
      get { return this.name; }
      set { this.name = value; this.resolvedType = null; }
    }
    IName name;

  }

  public sealed class NestedAliasForType : AliasForType, INestedAliasForType, ICopyFrom<INestedAliasForType> {

    public NestedAliasForType() {
      this.containingAlias = Dummy.AliasForType;
      this.name = Dummy.Name;
      this.visibility = TypeMemberVisibility.Default;
    }

    public void Copy(INestedAliasForType nestedAliasForType, IInternFactory internFactory) {
      ((ICopyFrom<IAliasForType>)this).Copy(nestedAliasForType, internFactory);
      this.containingAlias = nestedAliasForType.ContainingAlias;
      this.name = nestedAliasForType.Name;
      this.visibility = nestedAliasForType.Visibility;
    }

    public IAliasForType ContainingAlias {
      get { return this.containingAlias; }
      set { this.containingAlias = value; }
    }
    IAliasForType containingAlias;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public IName Name {
      get { return this.name; }
      set { this.name = value; }
    }
    IName name;

    public TypeMemberVisibility Visibility {
      get { return this.visibility; }
      set { this.visibility = value; }
    }
    TypeMemberVisibility visibility;


    IScope<IAliasMember> IScopeMember<IScope<IAliasMember>>.ContainingScope {
      get { return this.ContainingAlias; }
    }

    IAliasForType IContainerMember<IAliasForType>.Container {
      get { return this.ContainingAlias; }
    }

  }

  public sealed class NestedTypeDefinition : TypeDefinition, INestedTypeDefinition, ICopyFrom<INestedTypeDefinition> {

    public NestedTypeDefinition() {
      this.containingTypeDefinition = Dummy.Type;
    }

    public void Copy(INestedTypeDefinition nestedTypeDefinition, IInternFactory internFactory) {
      ((ICopyFrom<INamedTypeDefinition>)this).Copy(nestedTypeDefinition, internFactory);
      this.containingTypeDefinition = nestedTypeDefinition.ContainingTypeDefinition;
      this.Visibility = nestedTypeDefinition.Visibility;
    }

    public ITypeDefinition ContainingTypeDefinition {
      get { return this.containingTypeDefinition; }
      set { this.containingTypeDefinition = value; }
    }
    ITypeDefinition containingTypeDefinition;

    public TypeMemberVisibility Visibility {
      get { return ((TypeMemberVisibility)this.flags) & TypeMemberVisibility.Mask; }
      set {
        this.flags &= (TypeDefinition.Flags)~TypeMemberVisibility.Mask;
        this.flags |= (TypeDefinition.Flags)(value & TypeMemberVisibility.Mask);
      }
    }

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    #region IContainerMember<ITypeDefinition> Members

    ITypeDefinition IContainerMember<ITypeDefinition>.Container {
      get { return this.ContainingTypeDefinition; }
    }

    #endregion

    #region IScopeMember<IScope<ITypeDefinitionMember>> Members

    IScope<ITypeDefinitionMember> IScopeMember<IScope<ITypeDefinitionMember>>.ContainingScope {
      get { return this.ContainingTypeDefinition; }
    }

    #endregion

    #region ITypeMemberReference Members

    ITypeReference ITypeMemberReference.ContainingType {
      get { return this.ContainingTypeDefinition; }
    }

    #endregion

    #region INestedTypeReference Members

    INestedTypeDefinition INestedTypeReference.ResolvedType {
      get { return this; }
    }

    #endregion

    #region ITypeMemberReference Members

    public ITypeDefinitionMember ResolvedTypeDefinitionMember {
      get { return this; }
    }

    #endregion
  }

  public class NestedTypeReference : TypeReference, INestedTypeReference, ICopyFrom<INestedTypeReference> {

    public NestedTypeReference() {
      this.containingType = Dummy.TypeReference;
      this.genericParameterCount = 0;
      this.mangleName = true;
      this.name = Dummy.Name;
    }

    public void Copy(INestedTypeReference nestedTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(nestedTypeReference, internFactory);
      this.containingType = nestedTypeReference.ContainingType;
      this.genericParameterCount = nestedTypeReference.GenericParameterCount;
      this.mangleName = nestedTypeReference.MangleName;
      this.name = nestedTypeReference.Name;
    }

    public ITypeReference ContainingType {
      get { return this.containingType; }
      set { this.containingType = value; this.resolvedType = null; }
    }
    ITypeReference containingType;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public ushort GenericParameterCount {
      get { return this.genericParameterCount; }
      set { this.genericParameterCount = value; this.resolvedType = null; }
    }
    ushort genericParameterCount;

    public bool MangleName {
      get { return this.mangleName; }
      set { this.mangleName = value; }
    }
    bool mangleName;

    public IName Name {
      get { return this.name; }
      set { this.name = value; this.resolvedType = null; }
    }
    IName name;

    private INestedTypeDefinition Resolve() {
      foreach (ITypeDefinitionMember member in this.containingType.ResolvedType.GetMembersNamed(this.name, false)) {
        INestedTypeDefinition/*?*/ neType = member as INestedTypeDefinition;
        if (neType != null && neType.GenericParameterCount == this.genericParameterCount) return neType;
      }
      return Dummy.NestedType;
    }

    public override ITypeDefinition ResolvedType {
      get { return ((INestedTypeReference)this).ResolvedType; }
    }

    INamedTypeDefinition INamedTypeReference.ResolvedType {
      get { return ((INestedTypeReference)this).ResolvedType; }
    }

    INestedTypeDefinition INestedTypeReference.ResolvedType {
      get {
        if (this.resolvedType == null)
          this.resolvedType = this.Resolve();
        return this.resolvedType;
      }
    }
    INestedTypeDefinition/*?*/ resolvedType;


    #region ITypeMemberReference Members

    public ITypeDefinitionMember ResolvedTypeDefinitionMember {
      get { return ((INestedTypeReference)this).ResolvedType; }
    }

    #endregion
  }

  public sealed class PointerTypeReference : TypeReference, IPointerTypeReference, ICopyFrom<IPointerTypeReference> {

    public PointerTypeReference() {
      this.targetType = Dummy.TypeReference;
    }

    public void Copy(IPointerTypeReference pointerTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(pointerTypeReference, internFactory);
      this.targetType = pointerTypeReference.TargetType;
    }

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    IPointerType ResolvedPointerType {
      get {
        if (this.resolvedType == null)
          this.resolvedType = PointerType.GetPointerType(this.targetType, this.InternFactory);
        return this.resolvedType;
      }
    }
    IPointerType/*?*/ resolvedType;

    public override ITypeDefinition ResolvedType {
      get { return this.ResolvedPointerType; }
    }

    public ITypeReference TargetType {
      get { return this.targetType; }
      set { this.targetType = value; this.resolvedType = null; }
    }
    ITypeReference targetType;

  }

  public sealed class SpecializedNestedTypeReference : NestedTypeReference, ISpecializedNestedTypeReference, ICopyFrom<ISpecializedNestedTypeReference> {

    public SpecializedNestedTypeReference() {
      this.unspecializedVersion = Dummy.NestedType;
    }

    public void Copy(ISpecializedNestedTypeReference specializedNestedTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<INestedTypeReference>)this).Copy(specializedNestedTypeReference, internFactory);
      this.unspecializedVersion = specializedNestedTypeReference.UnspecializedVersion;
    }

    public INestedTypeReference UnspecializedVersion {
      get { return this.unspecializedVersion; }
      set { this.unspecializedVersion = value; }
    }
    INestedTypeReference unspecializedVersion;

  }

  public abstract class TypeDefinition : INamedTypeDefinition, ICopyFrom<INamedTypeDefinition> {

    internal TypeDefinition() {
      this.alignment = 0;
      this.attributes = new List<ICustomAttribute>();
      this.baseClasses = new List<ITypeReference>();
      this.explicitImplementationOverrides = new List<IMethodImplementation>();
      this.events = new List<IEventDefinition>();
      this.fields = new List<IFieldDefinition>();
      this.genericParameters = new List<IGenericTypeParameter>();
      this.interfaces = new List<ITypeReference>();
      this.internFactory = Dummy.InternFactory;
      this.layout = LayoutKind.Auto;
      this.locations = new List<ILocation>();
      this.MangleName = true;
      this.methods = new List<IMethodDefinition>();
      this.name = Dummy.Name;
      this.nestedTypes = new List<INestedTypeDefinition>();
      this.platformType = Dummy.PlatformType;
      this.privateHelperMembers = null;
      this.properties = new List<IPropertyDefinition>();
      this.securityAttributes = new List<ISecurityAttribute>();
      this.sizeOf = 0;
      this.stringFormat = StringFormatKind.Ansi;
      this.template = Dummy.Type;
      this.typeCode = PrimitiveTypeCode.NotPrimitive;
      this.underlyingType = Dummy.Type;
    }

    public void Copy(INamedTypeDefinition typeDefinition, IInternFactory internFactory) {
      this.alignment = typeDefinition.Alignment;
      this.attributes = new List<ICustomAttribute>(typeDefinition.Attributes);
      this.baseClasses = new List<ITypeReference>(typeDefinition.BaseClasses);
      this.events = new List<IEventDefinition>(typeDefinition.Events);
      this.explicitImplementationOverrides = new List<IMethodImplementation>(typeDefinition.ExplicitImplementationOverrides);
      this.fields = new List<IFieldDefinition>(typeDefinition.Fields);
      if (typeDefinition.IsGeneric)
        this.genericParameters = new List<IGenericTypeParameter>(typeDefinition.GenericParameters);
      else
        this.genericParameters = new List<IGenericTypeParameter>(0);
      this.interfaces = new List<ITypeReference>(typeDefinition.Interfaces);
      this.internFactory = internFactory;
      this.layout = typeDefinition.Layout;
      this.locations = new List<ILocation>(typeDefinition.Locations);
      this.methods = new List<IMethodDefinition>(typeDefinition.Methods);
      this.name = typeDefinition.Name;
      this.nestedTypes = new List<INestedTypeDefinition>(typeDefinition.NestedTypes);
      this.platformType = typeDefinition.PlatformType;
      this.privateHelperMembers = null;
      this.properties = new List<IPropertyDefinition>(typeDefinition.Properties);
      if (typeDefinition.HasDeclarativeSecurity)
        this.securityAttributes = new List<ISecurityAttribute>(typeDefinition.SecurityAttributes);
      else
        this.securityAttributes = new List<ISecurityAttribute>(0);
      this.sizeOf = typeDefinition.SizeOf;
      this.stringFormat = typeDefinition.StringFormat;
      this.template = typeDefinition;
      this.typeCode = typeDefinition.TypeCode;
      if (typeDefinition.IsEnum)
        this.underlyingType = typeDefinition.UnderlyingType;
      else
        this.underlyingType = Dummy.Type;
      //^ base();
      this.HasDeclarativeSecurity = typeDefinition.HasDeclarativeSecurity;
      this.IsAbstract = typeDefinition.IsAbstract;
      this.IsBeforeFieldInit = typeDefinition.IsBeforeFieldInit;
      this.IsClass = typeDefinition.IsClass;
      this.IsComObject = typeDefinition.IsComObject;
      this.IsDelegate = typeDefinition.IsDelegate;
      this.IsEnum = typeDefinition.IsEnum;
      this.IsInterface = typeDefinition.IsInterface;
      this.IsRuntimeSpecial = typeDefinition.IsRuntimeSpecial;
      this.IsSealed = typeDefinition.IsSealed;
      this.IsSerializable = typeDefinition.IsSerializable;
      this.IsSpecialName = typeDefinition.IsSpecialName;
      this.IsStatic = typeDefinition.IsStatic;
      this.IsStruct = typeDefinition.IsStruct;
      this.MangleName = typeDefinition.MangleName;
      if (typeDefinition.IsValueType) this.flags |= Flags.ValueType;
    }

    public virtual ushort Alignment {
      get { return this.alignment; }
      set { this.alignment = value; }
    }
    ushort alignment;

    public List<ICustomAttribute> Attributes {
      get { return this.attributes; }
      set { this.attributes = value; }
    }
    List<ICustomAttribute> attributes;

    public virtual List<ITypeReference> BaseClasses {
      get { return this.baseClasses; }
      set { this.baseClasses = value; }
    }
    List<ITypeReference> baseClasses;

    //^ [Pure]
    public bool Contains(ITypeDefinitionMember member) {
      foreach (ITypeDefinitionMember tdmem in this.Members)
        if (member == tdmem) return true;
      return false;
    }

    /// <summary>
    /// Calls the visitor.Visit(T) method where T is the most derived object model node interface type implemented by the concrete type
    /// of the object implementing IDoubleDispatcher. The dispatch method does not invoke Dispatch on any child objects. If child traversal
    /// is desired, the implementations of the Visit methods should do the subsequent dispatching.
    /// </summary>
    public abstract void Dispatch(IMetadataVisitor visitor);

    public List<IEventDefinition> Events {
      get { return this.events; }
      set { this.events = value; }
    }
    List<IEventDefinition> events;

    public List<IMethodImplementation> ExplicitImplementationOverrides {
      get { return this.explicitImplementationOverrides; }
      set { this.explicitImplementationOverrides = value; }
    }
    List<IMethodImplementation> explicitImplementationOverrides;

    public List<IFieldDefinition> Fields {
      get { return this.fields; }
      set { this.fields = value; }
    }
    List<IFieldDefinition> fields;

    [Flags]
    internal enum Flags {
      Abstract=0x40000000,
      Class=0x20000000,
      Delegate=0x10000000,
      Enum=0x08000000,
      HasDeclarativeSecurity=0x04000000,
      Interface=0x02000000,
      Sealed=0x01000000,
      Static=0x00800000,
      Struct=0x00400000,
      ValueType=0x00200000,
      IsRuntimeSpecialName=0x00100000,
      IsSpecialName=0x00080000,
      IsComObject=0x00040000,
      IsSerializable=0x00020000,
      IsBeforeFieldInit=0x00010000,
      MustBeReferenceType=0x00008000,
      MustBeValueType=0x00004000,
      MustHaveDefaultConstructor=0x00002000,
      MangleName = 0x00001000,
      None=0x00000000,
    }
    internal Flags flags;

    public virtual List<IGenericTypeParameter> GenericParameters {
      get { return this.genericParameters; }
      set { this.genericParameters = value; }
    }
    List<IGenericTypeParameter> genericParameters;

    public ushort GenericParameterCount {
      get { return (ushort)this.GenericParameters.Count; }
    }

    //^ [Pure]
    public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate) {
      foreach (ITypeDefinitionMember tdmem in this.Members) {
        if (tdmem.Name.UniqueKey == name.UniqueKey || ignoreCase && (name.UniqueKeyIgnoringCase == tdmem.Name.UniqueKeyIgnoringCase)) {
          if (predicate(tdmem)) yield return tdmem;
        }
      }
    }

    //^ [Pure]
    public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate) {
      foreach (ITypeDefinitionMember tdmem in this.Members) {
        if (predicate(tdmem)) yield return tdmem;
      }
    }

    //^ [Pure]
    public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase) {
      foreach (ITypeDefinitionMember tdmem in this.Members) {
        if (tdmem.Name.UniqueKey == name.UniqueKey || ignoreCase && (name.UniqueKeyIgnoringCase == tdmem.Name.UniqueKeyIgnoringCase)) {
          yield return tdmem;
        }
      }
    }

    public bool HasDeclarativeSecurity {
      get { return (this.flags & Flags.HasDeclarativeSecurity) != 0; }
      set {
        if (value)
          this.flags |= Flags.HasDeclarativeSecurity;
        else
          this.flags &= ~Flags.HasDeclarativeSecurity;
      }
    }

    public IGenericTypeInstanceReference InstanceType {
      get {
        if (this.instanceType == null) {
          lock (GlobalLock.LockingObject) {
            if (this.instanceType == null) {
              List<ITypeReference> arguments = new List<ITypeReference>();
              foreach (IGenericTypeParameter gpar in this.GenericParameters) arguments.Add(gpar);
              this.instanceType = GenericTypeInstance.GetGenericTypeInstance(this, arguments, this.InternFactory);
            }
          }
        }
        return this.instanceType;
      }
    }
    IGenericTypeInstanceReference/*?*/ instanceType;
    //^ invariant instanceType == null || !instanceType.IsGeneric;

    public virtual List<ITypeReference> Interfaces {
      get { return this.interfaces; }
      set { this.interfaces = value; }
    }
    List<ITypeReference> interfaces;

    public bool IsAbstract {
      get { return (this.flags & Flags.Abstract) != 0; }
      set {
        if (value)
          this.flags |= Flags.Abstract;
        else
          this.flags &= ~Flags.Abstract;
      }
    }

    public bool IsClass {
      get { return (this.flags & Flags.Class) != 0; }
      set {
        if (value)
          this.flags |= Flags.Class;
        else
          this.flags &= ~Flags.Class;
      }
    }

    public bool IsDelegate {
      get { return (this.flags & Flags.Delegate) != 0; }
      set {
        if (value)
          this.flags |= Flags.Delegate;
        else
          this.flags &= ~Flags.Delegate;
      }
    }

    public bool IsEnum {
      get { return (this.flags & Flags.Enum) != 0; }
      set {
        if (value)
          this.flags |= Flags.Enum;
        else
          this.flags &= ~Flags.Enum;
      }
    }

    public bool IsGeneric {
      get { return this.GenericParameters.Count > 0; }
    }

    public bool IsInterface {
      get { return (this.flags & Flags.Interface) != 0; }
      set {
        if (value)
          this.flags |= Flags.Interface;
        else
          this.flags &= ~Flags.Interface;
      }
    }

    public virtual bool IsReferenceType {
      get { return (this.flags & (Flags.Enum|Flags.ValueType|Flags.Static)) == 0; }
    }

    public bool IsSealed {
      get { return (this.flags & Flags.Sealed) != 0; }
      set {
        if (value)
          this.flags |= Flags.Sealed;
        else
          this.flags &= ~Flags.Sealed;
      }
    }

    public bool IsStatic {
      get { return (this.flags & Flags.Static) != 0; }
      set {
        if (value)
          this.flags |= Flags.Static;
        else
          this.flags &= ~Flags.Static;
      }
    }

    public virtual bool IsValueType {
      get { return (this.flags & Flags.ValueType) != 0; }
      set {
        if (value)
          this.flags |= Flags.ValueType;
        else
          this.flags &= ~Flags.ValueType;
      }
    }

    public bool IsRuntimeSpecial {
      get { return (this.flags & Flags.IsRuntimeSpecialName) != 0; }
      set {
        if (value)
          this.flags |= Flags.IsRuntimeSpecialName;
        else
          this.flags &= ~Flags.IsRuntimeSpecialName;
      }
    }

    public bool IsStruct {
      get { return (this.flags & Flags.Struct) != 0; }
      set {
        if (value)
          this.flags |= Flags.Struct;
        else
          this.flags &= ~Flags.Struct;
      }
    }

    public bool IsSpecialName {
      get { return (this.flags & Flags.IsSpecialName) != 0; }
      set {
        if (value)
          this.flags |= Flags.IsSpecialName;
        else
          this.flags &= ~Flags.IsSpecialName;
      }
    }

    public bool IsComObject {
      get { return (this.flags & Flags.IsComObject) != 0; }
      set {
        if (value)
          this.flags |= Flags.IsComObject;
        else
          this.flags &= ~Flags.IsComObject;
      }
    }

    public bool IsSerializable {
      get { return (this.flags & Flags.IsSerializable) != 0; }
      set {
        if (value)
          this.flags |= Flags.IsSerializable;
        else
          this.flags &= ~Flags.IsSerializable;
      }
    }

    public bool IsBeforeFieldInit {
      get { return (this.flags & Flags.IsBeforeFieldInit) != 0; }
      set {
        if (value)
          this.flags |= Flags.IsBeforeFieldInit;
        else
          this.flags &= ~Flags.IsBeforeFieldInit;
      }
    }

    public LayoutKind Layout {
      get { return this.layout; }
      set { this.layout = value; }
    }
    LayoutKind layout;

    public List<ILocation> Locations {
      get { return this.locations; }
      set { this.locations = value; }
    }
    List<ILocation> locations;

    public bool MangleName {
      get { return (this.flags & Flags.MangleName) != 0; }
      set {
        if (value)
          this.flags |= Flags.MangleName;
        else
          this.flags &= ~Flags.MangleName;
      }
    }

    public IEnumerable<ITypeDefinitionMember> Members {
      get {
        foreach (IEventDefinition eventDefinition in this.events)
          yield return eventDefinition;
        foreach (IFieldDefinition fieldDefinition in this.fields)
          yield return fieldDefinition;
        foreach (IMethodDefinition methodDefinition in this.methods)
          yield return methodDefinition;
        foreach (INestedTypeDefinition nestedTypeDefinition in this.nestedTypes)
          yield return nestedTypeDefinition;
        foreach (IPropertyDefinition propertyDefinition in this.properties)
          yield return propertyDefinition;
      }
    }

    public List<IMethodDefinition> Methods {
      get { return this.methods; }
      set { this.methods = value; }
    }
    List<IMethodDefinition> methods;

    public IName Name {
      get { return this.name; }
      set { this.name = value; }
    }
    IName name;

    public List<INestedTypeDefinition> NestedTypes {
      get { return this.nestedTypes; }
      set { this.nestedTypes = value; }
    }
    List<INestedTypeDefinition> nestedTypes;

    public IPlatformType PlatformType {
      get { return this.platformType; }
      set { this.platformType = value; }
    }
    IPlatformType platformType;

    public List<ITypeDefinitionMember> PrivateHelperMembers {
      get {
        if (this.privateHelperMembers == null) {
          this.privateHelperMembers = new List<ITypeDefinitionMember>(this.template.PrivateHelperMembers);
          this.template = Dummy.Type;
        }
        return this.privateHelperMembers; 
      }
      set { this.privateHelperMembers = value; }
    }
    List<ITypeDefinitionMember>/*?*/ privateHelperMembers;
    ITypeDefinition template;

    public List<IPropertyDefinition> Properties {
      get { return this.properties; }
      set { this.properties = value; }
    }
    List<IPropertyDefinition> properties;

    public List<ISecurityAttribute> SecurityAttributes {
      get { return this.securityAttributes; }
      set { this.securityAttributes = value; }
    }
    List<ISecurityAttribute> securityAttributes;

    public virtual uint SizeOf {
      get { return this.sizeOf; }
      set { this.sizeOf = value; }
    }
    uint sizeOf;

    public StringFormatKind StringFormat {
      get { return this.stringFormat; }
      set { this.stringFormat = value; }
    }
    StringFormatKind stringFormat;

    public override string ToString() {
      return TypeHelper.GetTypeName(this);
    }

    public virtual PrimitiveTypeCode TypeCode {
      get { return this.typeCode; }
      set { this.typeCode = value; }
    }
    PrimitiveTypeCode typeCode;

    public ITypeReference UnderlyingType {
      get { return this.underlyingType; }
      set { this.underlyingType = value; }
    }
    ITypeReference underlyingType;

    #region ITypeDefinition Members

    IEnumerable<IGenericTypeParameter> ITypeDefinition.GenericParameters {
      get { return this.GenericParameters.AsReadOnly(); }
    }

    IEnumerable<ITypeReference> ITypeDefinition.BaseClasses {
      get { return this.BaseClasses.AsReadOnly(); }
    }

    IEnumerable<IEventDefinition> ITypeDefinition.Events {
      get { return this.Events.AsReadOnly(); }
    }

    IEnumerable<IMethodImplementation> ITypeDefinition.ExplicitImplementationOverrides {
      get { return this.ExplicitImplementationOverrides.AsReadOnly(); }
    }

    IEnumerable<IFieldDefinition> ITypeDefinition.Fields {
      get { return this.Fields.AsReadOnly(); }
    }

    IEnumerable<ITypeReference> ITypeDefinition.Interfaces {
      get { return this.Interfaces.AsReadOnly(); }
    }

    IEnumerable<IMethodDefinition> ITypeDefinition.Methods {
      get { return this.Methods.AsReadOnly(); }
    }

    IEnumerable<INestedTypeDefinition> ITypeDefinition.NestedTypes {
      get { return this.NestedTypes.AsReadOnly(); }
    }

    IEnumerable<ITypeDefinitionMember> ITypeDefinition.PrivateHelperMembers {
      get { return this.PrivateHelperMembers.AsReadOnly(); }
    }

    IEnumerable<IPropertyDefinition> ITypeDefinition.Properties {
      get { return this.Properties.AsReadOnly(); }
    }

    IEnumerable<ISecurityAttribute> ITypeDefinition.SecurityAttributes {
      get { return this.SecurityAttributes.AsReadOnly(); }
    }
    #endregion

    #region IReference Members

    IEnumerable<ICustomAttribute> IReference.Attributes {
      get { return this.attributes.AsReadOnly(); }
    }

    IEnumerable<ILocation> IReference.Locations {
      get { return this.locations.AsReadOnly(); }
    }

    #endregion

    #region ITypeReference Members

    public bool IsAlias {
      get { return false; }
    }

    public IAliasForType AliasForType {
      get { return Dummy.AliasForType; }
    }

    public IEnumerable<ICustomModifier> CustomModifiers {
      get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
    }

    public bool IsModified {
      get { return false; }
    }

    public INamedTypeDefinition ResolvedType {
      get { return this; }
    }

    #endregion

    #region ITypeReference Members

    public uint InternedKey {
      get { return this.InternFactory.GetTypeReferenceInternedKey(this); }
    }

    ITypeDefinition ITypeReference.ResolvedType {
      get { return this.ResolvedType; }
    }

    #endregion

    public IInternFactory InternFactory {
      get { return this.internFactory; }
      set { this.internFactory = value; }
    }
    IInternFactory internFactory;

  }

  public sealed class ModifiedTypeReference : TypeReference, IModifiedTypeReference, ICopyFrom<IModifiedTypeReference> {

    public ModifiedTypeReference() {
      this.customModifiers = new List<ICustomModifier>();
      this.unmodifiedType = Dummy.TypeReference;
    }

    public void Copy(IModifiedTypeReference modifiedTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(modifiedTypeReference, internFactory);
      this.customModifiers = new List<ICustomModifier>(modifiedTypeReference.CustomModifiers);
      this.unmodifiedType = modifiedTypeReference.UnmodifiedType;
    }

    public List<ICustomModifier> CustomModifiers {
      get { return this.customModifiers; }
      set { this.customModifiers = value; }
    }
    List<ICustomModifier> customModifiers;

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public override ITypeDefinition ResolvedType {
      get { return this.unmodifiedType.ResolvedType; }
    }

    public ITypeReference UnmodifiedType {
      get { return this.unmodifiedType; }
      set { this.unmodifiedType = value; }
    }
    ITypeReference unmodifiedType;

    #region IModifiedTypeReference Members

    IEnumerable<ICustomModifier> IModifiedTypeReference.CustomModifiers {
      get { return this.customModifiers.AsReadOnly(); }
    }

    #endregion

  }

  public abstract class TypeReference : ITypeReference, ICopyFrom<ITypeReference> {

    internal TypeReference() {
      this.aliasForType = Dummy.AliasForType;
      this.attributes = new List<ICustomAttribute>();
      this.internFactory = Dummy.InternFactory;
      this.isEnum = false;
      this.isValueType = false;
      this.locations = new List<ILocation>();
      this.platformType = Dummy.PlatformType;
      this.typeCode = PrimitiveTypeCode.Invalid;
    }

    public void Copy(ITypeReference typeReference, IInternFactory internFactory) {
      this.aliasForType = typeReference.AliasForType;
      this.attributes = new List<ICustomAttribute>(typeReference.Attributes);
      this.internFactory = internFactory;
      this.isEnum = typeReference.IsEnum;
      this.isValueType = typeReference.IsValueType;
      this.locations = new List<ILocation>(typeReference.Locations);
      this.platformType = typeReference.PlatformType;
      this.typeCode = typeReference.TypeCode;
      this.originalReference = typeReference;
    }

    ITypeReference/*?*/ originalReference;

    public IAliasForType AliasForType {
      get { return this.aliasForType; }
      set { this.aliasForType = value; }
    }
    IAliasForType aliasForType;

    public List<ICustomAttribute> Attributes {
      get { return this.attributes; }
      set { this.attributes = value; }
    }
    List<ICustomAttribute> attributes;

    public abstract void Dispatch(IMetadataVisitor visitor);

    public IInternFactory InternFactory {
      get { return this.internFactory; }
      set { this.internFactory = value; }
    }
    IInternFactory internFactory;

    public uint InternedKey {
      get { return this.internFactory.GetTypeReferenceInternedKey(this); }
    }

    public bool IsAlias {
      get { return this.aliasForType != Dummy.AliasForType; }
    }

    public bool IsEnum {
      get { return this.isEnum; }
      set { this.isEnum = value; }
    }
    bool isEnum;

    public bool IsValueType {
      get {
        if (this.originalReference != null) return this.originalReference.IsValueType;
        return this.isValueType; 
      }
      set {
        this.originalReference = null;
        this.isValueType = value; 
      }
    }
    bool isValueType;

    public abstract ITypeDefinition ResolvedType {
      get;
    }

    public List<ILocation> Locations {
      get { return this.locations; }
      set { this.locations = value; }
    }
    List<ILocation> locations;

    public IPlatformType PlatformType {
      get { return this.platformType; }
      set { this.platformType = value; }
    }
    IPlatformType platformType;

    public override string ToString() {
      return TypeHelper.GetTypeName(this);
    }

    public virtual PrimitiveTypeCode TypeCode {
      get { return this.typeCode; }
      set { this.typeCode = value; }
    }
    PrimitiveTypeCode typeCode;

    #region IReference Members

    IEnumerable<ICustomAttribute> IReference.Attributes {
      get { return this.attributes.AsReadOnly(); }
    }

    IEnumerable<ILocation> IReference.Locations {
      get { return this.locations.AsReadOnly(); }
    }

    #endregion
  }

  public sealed class VectorTypeReference : TypeReference, IArrayTypeReference, ICopyFrom<IArrayTypeReference> {

    public VectorTypeReference() {
      this.elementType = Dummy.TypeReference;
    }

    public void Copy(IArrayTypeReference vectorTypeReference, IInternFactory internFactory) {
      ((ICopyFrom<ITypeReference>)this).Copy(vectorTypeReference, internFactory);
      this.elementType = vectorTypeReference.ElementType;
    }

    public override void Dispatch(IMetadataVisitor visitor) {
      visitor.Visit(this);
    }

    public ITypeReference ElementType {
      get { return this.elementType; }
      set { this.elementType = value; }
    }
    ITypeReference elementType;

    public bool IsVector {
      get { return true; }
    }

    public IEnumerable<int> LowerBounds {
      get { return IteratorHelper.GetEmptyEnumerable<int>(); }
    }

    public uint Rank {
      get { return 1; }
    }

    public IEnumerable<ulong> Sizes {
      get { return IteratorHelper.GetEmptyEnumerable<ulong>(); }
    }

    public override ITypeDefinition ResolvedType {
      get { return this.ResolvedArrayType; }
    }

    IArrayType ResolvedArrayType {
      get {
        if (this.resolvedType == null)
          this.resolvedType = Vector.GetVector(this.ElementType, this.InternFactory);
        return this.resolvedType;
      }
    }
    IArrayType/*?*/ resolvedType;

  }

}

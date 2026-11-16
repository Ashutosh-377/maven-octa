# MAVEN-OCTA

### An Architecture Built from First Principles.

**MAVEN-OCTA** is an independently designed 8-bit computer architecture featuring a 16-bit instruction format, implemented as a software-based processor emulator using C# and .NET.

It is an exploration of computer architecture beyond the use of established instruction sets. From defining the instruction encoding to implementing privilege boundaries, memory organisation, system calls, and an assembler, MAVEN-OCTA develops its own architectural model and execution semantics.

The objective is not simply to emulate a processor, but to design one — defining how its instructions behave, how its memory is organised, and how execution is controlled.

---

## Architecture at a Glance

| Property | Specification |
|:---|:---|
| Architecture | Custom 8-bit architecture |
| Data width | 8 bits |
| Instruction width | 16 bits |
| Address width | 8 bits |
| Addressable memory | 256 bytes |
| General-purpose registers | 8 (R0–R7) |
| Primary opcode width | 4 bits |
| Execution modes | User Mode / Kernel Mode |
| Instruction mechanism | Primary + Extended |
| Implementation language | C# |
| Runtime | .NET |
| Project category | ISA / Processor Emulator / Assembler |
| Development approach | Independent architectural design |

## Core Architectural Features

MAVEN-OCTA brings together several independently designed components into a single processor execution environment.

**Instruction Architecture**
- Custom 16-bit instruction encoding.
- 4-bit primary opcode field.
- Extended instruction mechanism.
- Arithmetic, logical, data-transfer, comparison, and control-flow operations.

**Processor Organisation**
- Eight general-purpose 8-bit registers.
- Dedicated Arithmetic Logic Unit (ALU).
- Zero, Negative, Carry, and Overflow flags.
- Centralised instruction decoding and execution.

**Memory Architecture**
- 256-byte addressable memory.
- Explicit memory-region organisation.
- Separate user program, kernel, and stack regions.
- Defined stack boundary and operation checks.

**Execution and Privilege**
- User Mode and Kernel Mode.
- Mode-dependent execution restrictions.
- System-call mechanism through SVC.
- Protected user execution context.
- IRET-based return from privileged execution.

**Software Environment**
- Custom assembly language and assembler.
- Modular C# emulator implementation.
- Character-oriented input/output.
- Integrated instruction loading and execution.

---

## Memory Architecture

MAVEN-OCTA implements an 8-bit address space, providing a total of 256 addressable bytes.

| Address Range | Memory Region | Capacity |
|:---|:---|---:|
| `0x00–0xBF` | User Program | 192 bytes |
| `0xC0–0xDF` | Kernel | 32 bytes |
| `0xE0–0xFF` | Stack | 32 bytes |
| **Total** | | **256 bytes** |

The memory layout establishes explicit boundaries between user programs, privileged kernel execution, and stack operations.

**Stack design:** All 32 addresses in the stack region are usable. `0xFF` is an ordinary stack address and is not reserved for system-call context storage. The processor maintains the necessary system-call context internally.

---

## Instruction Set Architecture

The primary instruction field is 4 bits wide, providing 16 possible primary opcode values.

### Primary Instructions

| Opcode | Mnemonic | Operation |
|:---:|:---|:---|
| `0x0` | LOADI | Load immediate value |
| `0x1` | ADD | Addition |
| `0x2` | STORE | Store register value in memory |
| `0x3` | LOAD | Load value from memory |
| `0x4` | SUB | Subtraction |
| `0x5` | AND | Bitwise AND |
| `0x6` | OR | Bitwise OR |
| `0x7` | XOR | Bitwise XOR |
| `0x8` | NOT | Bitwise NOT |
| `0x9` | INC | Increment |
| `0xA` | DEC | Decrement |
| `0xB` | CMP | Comparison |
| `0xC` | JMP | Unconditional jump |
| `0xD` | JZ | Jump if Zero |
| `0xE` | JNZ | Jump if Not Zero |
| `0xF` | EXT | Extended instruction mechanism |

### Extended Instruction Space

The `EXT` opcode introduces a secondary instruction space, extending the architecture beyond its 16 primary opcode values.

| Instruction | Purpose |
|:---|:---|
| HALT | Halt processor execution |
| PUSH | Push a value onto the stack |
| POP | Retrieve a value from the stack |
| CALL | Transfer control to a procedure |
| RET | Return from a procedure |
| SVC | Request a kernel service |
| IRET | Return from privileged execution |
| IN | Character-oriented input |
| OUT | Character-oriented output |

The SVC instruction uses the encoding `0xF500`. The complete extended instruction encoding and operand definitions are maintained in the architecture specification.

---

## Execution and Privilege Model

MAVEN-OCTA defines two distinct execution environments.

### User Mode

- Executes user programs within the designated program memory region.
- Operates under user-mode execution restrictions.
- Can request privileged services through SVC.
- Transfers control to the kernel through the defined system-call mechanism.

### Kernel Mode

- Executes privileged kernel instructions.
- Handles system-call processing.
- Operates under kernel-mode memory and execution restrictions.
- Rejects SVC execution while already in Kernel Mode.

### System Calls and Context Management

The SVC mechanism provides a controlled transition from User Mode to Kernel Mode.

During the transition, the processor preserves the user return location in protected internal processor state. This mechanism does not consume or reserve any address in the stack region.

IRET restores the saved user execution context and returns execution to User Mode.

---

## Emulator Architecture

The software implementation is organised into modular components, each responsible for a defined part of processor operation.

| Component | Responsibility |
|:---|:---|
| `CPU` | Instruction execution and processor coordination |
| `Memory` | Memory storage and access |
| `MemoryMap` | Memory-region definitions and boundaries |
| `Instruction` | Instruction representation |
| `Opcode` | Opcode definitions |
| `Decoder` | Instruction decoding |
| `ALU` | Arithmetic and logical operations |
| `Registers` | General-purpose and processor register state |
| `Flags` | Processor status flags |

The assembler integrates with the execution environment, allowing assembly source code to be translated into machine instructions and executed by the processor emulator.

---

## Validation and Testing

MAVEN-OCTA has undergone regression testing, architectural boundary testing, and assembler integration testing throughout development.

Recorded validation includes:

- **10 passing regression test cases.**
- Stack boundary and invalid stack-operation checks.
- Procedure CALL and RET validation.
- Invalid kernel control-transfer checks.
- Odd-address jump validation.
- SVC and IRET transition validation.
- Rejection of SVC execution from Kernel Mode.
- Multi-line assembler integration testing.

### Recorded Integration Test

A successful assembler-to-processor integration test produced the following result:

```text
R0 = 30
R1 = 20
Memory[200] = 30
Halted = True

ALL TESTS PASSED
```

### Kernel Privilege Protection

A separate kernel protection test confirmed that attempting to execute SVC while already in Kernel Mode is rejected.

These results document specific completed tests. They do not imply exhaustive verification of every instruction, operand combination, or architectural boundary.

---

## Technical Documentation

**MAVEN-OCTA — Architecture and Implementation Specification**

A dedicated technical specification accompanies the software implementation.

The specification is organised into 20 chapters and two appendices, covering the architectural design, implementation, and validation of the processor.

Its subject areas include:

1. Architectural introduction and design philosophy
2. Data model and instruction organisation
3. Register architecture
4. Instruction encoding and instruction set
5. Arithmetic Logic Unit and processor flags
6. Memory architecture
7. Control flow and stack architecture
8. Extended instruction mechanisms
9. Execution modes and privilege boundaries
10. System calls and kernel architecture
11. Input/output mechanisms
12. Assembler design
13. Emulator implementation
14. Validation and testing

**Documentation release:** The technical specification is undergoing its final source-level corrections and verification. Its finalized release will be published separately.

---

## Development Status

The initial processor architecture, emulator, assembler integration, and kernel execution mechanisms have been implemented.

The software implementation is being prepared for its initial public release.

The accompanying technical specification is undergoing final review, including verification of instruction encodings, operand semantics, extended instruction behaviour, and implementation consistency.

The software release and finalized documentation release are intentionally being treated as separate milestones.

---

## Design Philosophy

MAVEN-OCTA is built around a simple idea:

**An architecture should be defined, not merely implemented.**

Every register, instruction, memory boundary, and execution transition represents a design decision. Each decision must have explicitly defined behaviour, an implementation, and a means of verification.

The project approaches processor design as an exercise in understanding the underlying principles of computation, instruction execution, and architectural organisation.

It is developed incrementally, prioritising architectural reasoning, implementation correctness, and verification over unnecessary complexity.

---

*Independently designed and implemented using C# and .NET.*

**MAVEN-OCTA — One instruction, one bug, and one correction at a time.**
